using Toolbelt.Blazor;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public class HttpInterceptorService
    (
        HttpClientInterceptor interceptor, 
        IAuthenticationService authenticationService
    )
    {
        public void RegisterEvent() => interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri!.AbsolutePath;

            if (!absPath.Contains("token") && !absPath.Contains("accounts") && !absPath.Contains("identity"))
            {
                Result<string> refreshResult = await authenticationService.RefreshAuthTokenAsync();

                if (!refreshResult.IsSuccess)
                {
                    // Handle token refresh failure, e.g., log out the user or show an error message
                    return;
                }

                string token = refreshResult.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                }
            }
        }

        public void DisposeEvent() => interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
    }
}
