using CloudAccounting.Wasm.Services.Repositories.Company;
using System;

namespace CloudAccounting.Wasm.Pages.Company
{
    public partial class CompaniesListPage
    {
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] public DialogService? DialogService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }        
        [Inject] private ILogger<CompaniesListPage>? Logger { get; set; }

        private List<CompanyDetail>? _companies;
        private bool isLoading;

        protected async override Task OnInitializedAsync() 
        {
            try 
            {
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

                // TODO: Remove log statement or change to Debug level
                Logger!.LogInformation("OnInitializedAsync -> Successfully retrieved {COUNT} companies.", _companies?.Count);
                await base.OnInitializedAsync();
            }
            catch (Exception ex) 
            {
                Logger!.LogError(ex, "An exception occurred while retrieving companies.");
                ShowErrorNotification.ShowError(
                    NotificationService!,
                    "An unexpected error occurred while retrieving companies."
                );

                Navigation?.NavigateTo("/");
            }
        }

        private async Task GetCompanies(LoadDataArgs args)
        {
            try
            {
                Result<List<CompanyDetail>> result = await CompanyService!.GetCompaniesAsync(args.Skip ?? default, args.Top ?? default);

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve companies: {ERROR}.", result.Error.Message);

                    ShowErrorNotification.ShowError(
                        NotificationService!,
                        result.Error.Message
                    );

                    Navigation?.NavigateTo("/");
                }

                isLoading = true;
                _companies = result.Value;
                isLoading = false;
                await InvokeAsync(StateHasChanged);

                // TODO: Remove log statement or change to Debug level
                Logger!.LogInformation("GetCompanies -> Successfully retrieved {COUNT} companies.", _companies?.Count);
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving companies.");
                ShowErrorNotification.ShowError(
                    NotificationService!,
                    "An unexpected error occurred while retrieving companies."
                );

                Navigation?.NavigateTo("/");
            }
        }

        private void ViewCompanyDetails(CompanyDetail model)
        {
            Navigation!.NavigateTo($"/Pages/Company/CompanyEditPage/{model.CompanyCode}");
        }
    }
}
