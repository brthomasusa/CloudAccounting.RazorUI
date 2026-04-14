using Blazored.LocalStorage;
using CloudAccounting.Wasm.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public class AuthenticationService
    (
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage,
        HttpClient httpClient

    ) : IAuthenticationService
    {
        const string uri = "/api/v1/identity/";

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginCommand request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{uri}login", request);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                    if (loginResponse is not null && loginResponse.Token != null)
                    {
                        await localStorage.SetItemAsync("authToken", loginResponse.Token);
                        await localStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken);
                        await localStorage.SetItemAsync("tokenExpiration", loginResponse.TokenExpired);

                        await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse.Token);

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

    }
}
