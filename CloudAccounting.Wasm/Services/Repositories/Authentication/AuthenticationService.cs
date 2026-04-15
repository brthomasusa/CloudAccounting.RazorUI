using Blazored.LocalStorage;
using CloudAccounting.Wasm.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public class AuthenticationService
    (
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage,
        IHttpClientFactory ClientFactory

    ) : IAuthenticationService
    {
        const string uri = "/api/v1/identity/";
        private readonly HttpClient _httpClient = ClientFactory.CreateClient("WithoutDelegateHandler");

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginCommand request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{uri}login", request);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                    if (loginResponse is not null && loginResponse.Token != null)
                    {
                        await localStorage.SetItemAsync("authToken", loginResponse.Token);
                        await localStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken);
                        await localStorage.SetItemAsync("tokenExpiration", loginResponse.TokenExpired);

                        await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                        return Result<LoginResponseModel>.Success(loginResponse);
                    }
                    else
                    {
                        return Result<LoginResponseModel>.Failure<LoginResponseModel>(
                            new Error("IdentityMgmtRepository.LoginUserAsync", "Invalid login attempt")
                        );
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    return Result<LoginResponseModel>.Failure<LoginResponseModel>(
                        new Error("IdentityMgmtRepository.LoginUserAsync", errorContent)
                    );
                }
            }
            catch (Exception ex)
            {
                return Result<LoginResponseModel>.Failure<LoginResponseModel>(
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
                var timeUTC = DateTime.UtcNow;
                var diff = expTime - timeUTC;

                if (diff.TotalMinutes <= 2)
                {
                    // Token is about to expire, attempt to refresh

                    var refreshToken = await localStorage.GetItemAsync<string>("refreshToken");

                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return Result<string>.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", "No refresh token found"));
                    }

                    var response = await _httpClient.PostAsJsonAsync($"{uri}loginbyrefreshtoken", new { RefreshToken = refreshToken });

                    if (response.IsSuccessStatusCode)
                    {
                        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                        if (loginResponse is not null && loginResponse.Token != null)
                        {
                            await localStorage.SetItemAsync("authToken", loginResponse.Token);
                            await localStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken);
                            await localStorage.SetItemAsync("tokenExpiration", loginResponse.TokenExpired);

                            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse.Token);

                            return loginResponse.Token;
                        }
                        else
                        {
                            return Result<string>.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", "Failed token refresh attempt"));
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Result<string>.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", errorContent));
                    }

                }
                else
                {
                    // Token is still valid
                    string? token = await localStorage.GetItemAsync<string>("authToken");
                    return Result<string>.Success(token!);
                }
            }
            catch (Exception ex)
            {
                return Result<string>.Failure<string>(new Error("IdentityMgmtRepository.RefreshAuthTokenAsync", ex.Message));
            }
        }
    }
}
