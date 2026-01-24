using CloudAccounting.Wasm.Services.Repositories.Company;
using Radzen.Blazor;
using System.Collections.ObjectModel;

namespace CloudAccounting.Wasm.Pages.Company
{
    public partial class CompanyEditPage
    {
        [Parameter] public int CompanyCode { get; set; }
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<CompanyEditPage>? Logger { get; set; }

        private CompanyDetail? _company;
        private string? _companyName;
        private static ReadOnlyCollection<string> _currencies = ["CAN", "USD"];
        private RadzenTemplateForm<CompanyDetail>? _companyDetailForm;

        protected override async Task OnParametersSetAsync()
        {
            Result<CompanyDetail> result = await CompanyService!.GetCompanyByIdAsync(CompanyCode);

            if (result.IsSuccess)
            {
                _company = result.Value;
                _companyName = result.Value.CompanyName;
            }
            else 
            {
                Logger!.LogError("Failed to retrieve company: {ERROR}.", result.Error.Message);

                ShowErrorNotification.ShowError(
                    NotificationService!,
                    result.Error.Message
                );

                Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
            }
        }

        private void Submit(CompanyDetail arg)
        {
            // manual validation
            // bool isValid = _companyDetailForm.EditContext.Validate();
        }

        private void Cancel()
        {
            Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
        }

        private void Delete()
        {
            Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
        }
    }
}
