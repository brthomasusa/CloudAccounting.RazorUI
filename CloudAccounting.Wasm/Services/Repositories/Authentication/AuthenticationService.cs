using Blazored.SessionStorage;
using CloudAccounting.Wasm.Authentication;
using CloudAccounting.Wasm.Models.Authentication;

using Microsoft.AspNetCore.Components.Authorization;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public class AuthenticationService
    (
        AuthenticationStateProvider authenticationStateProvider,
        IHttpClientFactory clientFactory,
        ISessionStorageService sessionStorage

    ) : IAuthenticationService
    {
        const string Uri = "/api/v1/identity/";
        private readonly HttpClient _httpClient = clientFactory.CreateClient("WithoutDelegateHandler");

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginCommand request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{Uri}login", request);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                    if (loginResponse is not null && loginResponse.Token != null)
                    {
                        await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse);

                        return Result.Success(loginResponse);
                    }
                    else
                    {
                        return Result.Failure<LoginResponseModel>(
                            new Error("IdentityMgmtRepository.LoginUserAsync", "Invalid login attempt")
                        );
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    return Result.Failure<LoginResponseModel>(
                        new Error("IdentityMgmtRepository.LoginUserAsync", errorContent)
                    );
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<LoginResponseModel>(
                    new Error("IdentityMgmtRepository.LoginUserAsync", ex.Message)
                );
            }
        }

        public async Task<Result<string>> RefreshAuthTokenAsync()
        {
            try
            {
                var authState = await ((CustomAuthStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
                var user = authState.User;

                var exp = user.FindFirst(c => c.Type.Equals("exp"))!.Value;
                var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
                var timeUtc = DateTime.UtcNow;
                var diff = expTime - timeUtc;

                if (diff.TotalMinutes <= 2)
                {
                    // Token is about to expire, attempt to refresh

                    var refreshToken = await sessionStorage.GetItemAsync<string>("refreshToken");

                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return Result.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", "No refresh token found"));
                    }

                    var response = await _httpClient.PostAsJsonAsync($"{Uri}loginbyrefreshtoken", new { RefreshToken = refreshToken });

                    if (response.IsSuccessStatusCode)
                    {
                        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                        if (loginResponse is not null && loginResponse.Token != null)
                        {
                            await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse);

                            return loginResponse.Token;
                        }
                        else
                        {
                            return Result.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", "Failed token refresh attempt"));
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Result.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", errorContent));
                    }

                }
                else
                {
                    // Token is still valid
                    string token = await sessionStorage.GetItemAsync<string>("authToken");
                    return Result.Success(token);
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", ex.Message));
            }
        }

        public async Task<Result<ApplicationUser>> GetUserByIdAsync(string userId)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{Uri}users/{userId}");

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ApplicationUser>();

                    if (response is not null)
                    {
                        return Result.Success(response);
                    }
                    else
                    {
                        return Result.Failure<ApplicationUser>(
                            new Error("IdentityMgmtRepository.GetUserByIdAsync", "User not found")
                        );
                    }
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure<ApplicationUser>(
                        new Error("IdentityMgmtRepository.GetUserByIdAsync", errorContent)
                    );
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<ApplicationUser>(
                    new Error("IdentityMgmtRepository.GetUserByIdAsync", ex.Message)
                );
            }
        }

        public async Task<Result<List<ApplicationUser>>> GetUsersByCompanyAndGroupAsync(int companyCode, int groupId)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{Uri}users/{companyCode}/{groupId}");

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<List<ApplicationUser>>();

                    if (response is not null)
                    {
                        return Result.Success(response);
                    }
                    else
                    {
                        return Result.Failure<List<ApplicationUser>>(
                            new Error("IdentityMgmtRepository.GetUsersByCompanyAsync", "No users found for the specified company and group")
                        );
                    }
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure<List<ApplicationUser>>(
                        new Error("IdentityMgmtRepository.GetUsersByCompanyAsync", errorContent)
                    );
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<List<ApplicationUser>>(
                    new Error("IdentityMgmtRepository.GetUsersByCompanyAsync", ex.Message)
                );
            }
        }

        public async Task<Result<List<RoleModel>>> GetAllRolesAsync()
        {
            try
            {
                var result = await _httpClient.GetAsync($"{Uri}roles");

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<List<RoleModel>>();

                    if (response is not null)
                    {
                        return Result.Success(response);
                    }
                    else
                    {
                        return Result.Failure<List<RoleModel>>(
                            new Error("IdentityMgmtRepository.GetAllRolesAsync", "No roles found")
                        );
                    }
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure<List<RoleModel>>(
                        new Error("IdentityMgmtRepository.GetAllRolesAsync", errorContent)
                    );
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<List<RoleModel>>(
                    new Error("IdentityMgmtRepository.GetAllRolesAsync", ex.Message)
                );
            }
        }

        public async Task<Result> CreateRoleAsync(RoleModel role)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"{Uri}roles", new { RoleName = role.GroupTitle });

                if (result.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure(new Error("IdentityMgmtRepository.CreateRoleAsync", errorContent));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("IdentityMgmtRepository.CreateRoleAsync", ex.Message));
            }
        }

        public async Task<Result> CreateUserWithRoleAsync(CreateUserWithRoleCommand command)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"{Uri}users/withrole", command);

                if (result.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure(new Error("IdentityMgmtRepository.CreateUserWithRoleAsync", errorContent));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("IdentityMgmtRepository.CreateUserWithRoleAsync", ex.Message));
            }
        }

        public async Task<Result> UpdateUserRoleAsync(UpdateUserRoleCommand command)
        {
            try
            {
                var result = await _httpClient.PutAsJsonAsync($"{Uri}users/updaterole", command);

                if (result.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    return Result.Failure(new Error("IdentityMgmtRepository.UpdateUserRoleAsync", errorContent));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("IdentityMgmtRepository.UpdateUserRoleAsync", ex.Message));
            }
        }
    }
}
