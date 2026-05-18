
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using CloudAccounting.Wasm;
using CloudAccounting.Wasm.Authentication;
using CloudAccounting.Wasm.Services.DelegatingHandlers;
using CloudAccounting.Wasm.Services.Repositories.Authentication;
using CloudAccounting.Wasm.Services.Repositories.Company;
using CloudAccounting.Wasm.Services.Repositories.CostCenters;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddTransient<RetryDelegatingHandler>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

Uri baseAddress = new(builder.Configuration["CloudAcctgApi"]!);

builder.Services.AddHttpClient("WithoutDelegateHandler", client =>
    client.BaseAddress = baseAddress);

builder.Services.AddHttpClient("CloudAccountingAPI", client =>
    client.BaseAddress = baseAddress)
        .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
        .AddHttpMessageHandler<RetryDelegatingHandler>();

builder.Services.AddScoped<ICompanyService, CompanyService>()
                .AddScoped<IVoucherTypeService, VoucherTypeService>()
                .AddScoped<ILookupService, LookupService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<ICostCenterService, CostCenterService>()
                .AddSingleton<IAuthenticatedUserState, AuthenticatedUserState>();

builder.Services.AddRadzenComponents()
                .AddScoped<DialogService>()
                .AddScoped<NotificationService>()
                .AddScoped<TooltipService>()
                .AddScoped<ContextMenuService>();

await builder.Build().RunAsync();