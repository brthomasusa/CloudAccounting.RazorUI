#pragma warning disable CS8604

using Blazored.SessionStorage;
using CloudAccounting.Wasm.Models.Authentication;
using CloudAccounting.Wasm.Models.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CloudAccounting.Wasm.Authentication
{
    public class CustomAuthStateProvider
    (
        ISessionStorageService sessionStorage,
        HttpClient httpClient
    ) : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient = httpClient;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string? authToken = await sessionStorage.GetItemAsync<string>("authToken");

                var identity = authToken == null ? new ClaimsIdentity() : GetClaimsIdentity(authToken);

                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                await MarkUserAsLoggedOut();
                var identity = new ClaimsIdentity();
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
        }

        public async Task MarkUserAsAuthenticated(LoginResponseModel loginResponse)
        {
            await sessionStorage.SetItemAsync("authToken", loginResponse.Token);
            await sessionStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken);

            var identity = GetClaimsIdentity(loginResponse.Token);
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private static ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task MarkUserAsLoggedOut()
        {
            await sessionStorage.RemoveItemAsync("authToken");
            await sessionStorage.RemoveItemAsync("refreshToken");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}