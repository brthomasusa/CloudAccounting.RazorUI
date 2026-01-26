using CloudAccounting.Wasm.Services.Repositories.Company;
using Microsoft.AspNetCore.Components.Routing;
using Radzen.Blazor;
using System.Collections.ObjectModel;

namespace CloudAccounting.Wasm.Pages.Company
{
    public partial class CompanyCreatePage
    {
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private DialogService? DialogService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<CompanyCreatePage>? Logger { get; set; }

        private CompanyDetail? _company = new();
        private static ReadOnlyCollection<string> _currencies = ["CAN", "USD"];
        private RadzenTemplateForm<CompanyDetail>? _companyDetailForm;
        private bool _hasUnsavedChanges = false;

        private async Task Submit(CompanyDetail arg)
        {
            // manual validation
            // bool isValid = _companyDetailForm.EditContext.Validate();

            _company!.Phone = PhoneNumberUtility.GetFormattedPhoneNumber(arg.Phone!);
            _company!.Fax = PhoneNumberUtility.GetFormattedPhoneNumber(arg.Fax!);

            Result<CompanyDetail> result = await CompanyService!.CreateCompanyAsync(arg);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "New company created",
                    Detail = $"Successfully created new company: {result.Value.CompanyName}.",
                    Duration = 4000
                });

                _hasUnsavedChanges = false;
            }
            else
            {
                Logger!.LogError("Failed to create company: {ERROR}.", result.Error.Message);

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
