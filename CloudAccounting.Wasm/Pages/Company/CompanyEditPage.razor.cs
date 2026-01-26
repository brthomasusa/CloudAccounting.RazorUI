using CloudAccounting.Wasm.Services.Repositories.Company;
using Microsoft.AspNetCore.Components.Routing;
using Radzen.Blazor;
using System.Collections.ObjectModel;

namespace CloudAccounting.Wasm.Pages.Company
{
    public partial class CompanyEditPage
    {
        [Parameter] public int CompanyCode { get; set; }
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private DialogService? DialogService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<CompanyEditPage>? Logger { get; set; }

        private CompanyDetail? _company;
        private string? _companyName;
        private static ReadOnlyCollection<string> _currencies = ["CAN", "USD"];
        private RadzenTemplateForm<CompanyDetail>? _companyDetailForm;
        private bool _hasUnsavedChanges = false;

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

        private async Task Submit(CompanyDetail arg)
        {
            // manual validation
            // bool isValid = _companyDetailForm.EditContext.Validate();

            _company!.Phone = PhoneNumberUtility.GetFormattedPhoneNumber(arg.Phone!);
            _company!.Fax = PhoneNumberUtility.GetFormattedPhoneNumber(arg.Fax!);

            Result result = await CompanyService!.UpdateCompanyAsync(arg);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Update succeeded",
                    Detail = $"Successfully updated information for {arg.CompanyName}.",
                    Duration = 4000
                });

                _hasUnsavedChanges = false;
            }
            else
            {
                Logger!.LogError("Failed to update company: {ERROR}.", result.Error.Message);

                ShowErrorNotification.ShowError(
                    NotificationService!,
                    result.Error.Message
                );                
            }

            Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
        }

        private async Task Cancel()
        {
            if (_hasUnsavedChanges)
            {
                string msg = "There are unsaved changes. Leave without saving?";
                var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

                if ((bool)dialogResponse)
                {
                    Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
                }
            }
            else
            {
                Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
            }
        }

        private async Task Delete()
        {
            Result result = await CompanyService!.DeleteCompanyAsync(_company!.CompanyCode);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Delete succeeded",
                    Detail = $"Successfully deleted {_company!.CompanyName}.",
                    Duration = 4000
                });

                _hasUnsavedChanges = false;
            }
            else
            {
                Logger!.LogError("Failed to delete company: {ERROR}.", result.Error.Message);

                ShowErrorNotification.ShowError(
                    NotificationService!,
                    result.Error.Message
                );
            }

            Navigation?.NavigateTo("/Pages/Company/CompaniesListPage");
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (context.IsNavigationIntercepted && _hasUnsavedChanges)
            {
                string msg = "There are unsaved changes. Leave without saving?";
                var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

                if (!(bool)dialogResponse)
                {
                    context.PreventNavigation();
                }
            }
        }
    }
}
