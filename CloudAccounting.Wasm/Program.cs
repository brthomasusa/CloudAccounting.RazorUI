
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CloudAccounting.Wasm;
using CloudAccounting.Wasm.Services.DelegatingHandlers;
using CloudAccounting.Wasm.Services.Repositories.Authentication;
using CloudAccounting.Wasm.Services.Repositories.Company;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using CloudAccounting.Wasm.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
Uri baseAddress = new(builder.Configuration["CloudAcctgApi"]!);

builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddTransient<RetryDelegatingHandler>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient("WithoutDelegateHandler", client =>
    client.BaseAddress = baseAddress);

builder.Services.AddHttpClient("CloudAccountingAPI", client =>
    client.BaseAddress = baseAddress)
        .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
        .AddHttpMessageHandler<RetryDelegatingHandler>();

builder.Services.AddScoped<ICompanyService, CompanyService>()
                .AddScoped<IVoucherTypeService, VoucherTypeService>()
                .AddScoped<ILookupService, LookupService>()
                .AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddRadzenComponents()
                .AddScoped<DialogService>()
                .AddScoped<NotificationService>()
                .AddScoped<TooltipService>()
                .AddScoped<ContextMenuService>();

await builder.Build().RunAsync();

//static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//{
//    return HttpPolicyExtensions
//        .HandleTransientHttpError()
//        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
//        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
//}