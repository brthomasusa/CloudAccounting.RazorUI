#pragma warning disable CS8604

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using CloudAccounting.Wasm.Models;
using System.IdentityModel.Tokens.Jwt;

namespace CloudAccounting.Wasm.Authentication
{
    public class CustomAuthStateProvider
    (
        ILocalStorageService localStorage,
        HttpClient httpClient
    ) : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient = httpClient;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var loginResponseModel = await localStorage.GetItemAsync<LoginResponseModel>("sessionState");

                var identity = loginResponseModel == null ? new ClaimsIdentity() : GetClaimsIdentity(loginResponseModel.Token);

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

        public async Task MarkUserAsAuthenticated(LoginResponseModel model)
        {
            await localStorage.SetItemAsync("sessionState", model);
            var identity = GetClaimsIdentity(model.Token);
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
            await localStorage.RemoveItemAsync("sessionState");
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }






    }
}