using CloudAccounting.Wasm.Models.Company;
using CloudAccounting.Wasm.Services.Repositories.Company;
using CloudAccounting.Wasm.Utilities;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace CloudAccounting.Wasm.Pages.Company
{
    public partial class CompaniesListPage
    {
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] public DialogService? DialogService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }

        private List<CompanyDetail>? _companies;
        [Inject] private ILogger<CompaniesListPage>? Logger { get; set; }

        protected async override Task OnInitializedAsync() 
        {
            await base.OnInitializedAsync();

            Result<List<CompanyDetail>> result = await CompanyService!.GetCompaniesAsync(1, 100);

            if (result.IsFailure) 
            {
                Logger!.LogError("Failed to retrieve companies: {ERROR}.", result.Error.Message);

                ShowErrorNotification.ShowError(
                    NotificationService!,
                    result.Error.Message
                );

                Navigation?.NavigateTo("/");
            }

            _companies = result.Value;
            Logger!.LogInformation("Successfully retrieved {COUNT} companies.", _companies?.Count);
        }
    }
}
