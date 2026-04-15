
using CloudAccounting.Wasm.Services.Repositories.Authentication;

namespace CloudAccounting.Wasm.Services.DelegatingHandlers
{
    public class AuthenticationDelegatingHandler
    (
        IAuthenticationService authenticationService, 
        ILogger<AuthenticationDelegatingHandler> logger
    ) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Result<string> result = await authenticationService.RefreshAuthTokenAsync();

            if (result.IsSuccess)
            {
                request.Headers.Add("Authorization", $"Bearer {result.Value}");
            }
            else
            {
                logger.LogError("Failed to refresh auth token: {ErrorMessage}", result.Error.Message);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
