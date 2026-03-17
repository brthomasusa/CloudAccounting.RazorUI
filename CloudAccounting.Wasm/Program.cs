
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Polly;
using Polly.Extensions.Http;
using CloudAccounting.Wasm;
using CloudAccounting.Wasm.Services.Repositories.Company;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("CloudAccountingApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CloudAcctgApi"]!);
    //client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
    //client.Timeout = TimeSpan.FromSeconds(30);
})
.SetHandlerLifetime(TimeSpan.FromSeconds(30))
.AddPolicyHandler(GetRetryPolicy());

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CloudAccountingApi"));

builder.Services.AddScoped<ICompanyService, CompanyService>()
                .AddScoped<IVoucherTypeService, VoucherTypeService>()
                .AddScoped<ILookupService, LookupService>();

builder.Services.AddRadzenComponents()
                .AddScoped<DialogService>()
                .AddScoped<NotificationService>()
                .AddScoped<TooltipService>()
                .AddScoped<ContextMenuService>();

await builder.Build().RunAsync();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}