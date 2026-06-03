using CloudAccounting.Wasm.Services.Repositories.Coa;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.Coa;

public partial class CoaEditPage
{
    [Parameter] public int CompanyCode { get; set; }
    [Parameter] public string AccountCode { get; set; } = string.Empty;
    [Inject] private ICoaService? CoaService { get; set; }
    [Inject] private ILookupService? LookupService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private DialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CoaEditPage>? Logger { get; set; }

    private List<CostCenterLookupItem>? _costCenterLookupItems;
    private ChartOfAccountDto? _chartOfAccountDto;
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _hasUnsavedChanges;

    protected override async Task OnInitializedAsync()
    {
        var result = await LookupService!.GetCostCenterLookups(CompanyCode);
        _costCenterLookupItems = result.Value;

        var coaResult = await CoaService!.RetrieveByIdAsync(CompanyCode, AccountCode);

        _chartOfAccountDto = coaResult.Value;

        await base.OnInitializedAsync();
    }

    private async Task Submit(ChartOfAccountDto arg)
    {
        try
        {
            UpdateChartOfAccountCommand command = new
            (
                arg.CompanyCode,
                arg.AccountCode,
                arg.AccountTitle!,
                arg.AccountType!,
                arg.CostCenterCode!
            );

            var result = await CoaService!.UpdateAsync(command);

            if (result.IsSuccess)
            {
                var message = $"GL account {arg.AccountTitle} updated successfully.";

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Update operation succeeded",
                    Detail = message,
                    Duration = 5000
                });

                _hasUnsavedChanges = false;

                Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
            }
            else
            {
                Logger!.LogError("Failed to update gl account: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Update operation failed";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while updating gl account.");

            _errorAlertTitle = "Update operation failed";
            _errorAlertMessage = "An unexpected error occurred while performing gl account update.";
            _showErrorAlert = true;

            Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
        }
    }

    private async Task Delete()
    {
        try
        {
            string msg = $"Do you wish to delete gl account {_chartOfAccountDto?.AccountCode}? This can't be undone!";
            var dialogResponse = await DialogService!.Confirm(msg, $"Delete {_chartOfAccountDto?.AccountTitle}?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if ((bool)dialogResponse!)
            {
                DeleteChartOfAccountCommand command = new(_chartOfAccountDto!.CompanyCode, _chartOfAccountDto?.AccountCode!);
                Result result = await CoaService!.DeleteAsync(command);

                if (result.IsSuccess)
                {
                    NotificationService!.Notify(new NotificationMessage
                    {
                        Style = "position: absolute; inset-inline-start: -1000px;",
                        Severity = NotificationSeverity.Success,
                        Summary = "Delete succeeded",
                        Detail = $"Successfully deleted gl account {_chartOfAccountDto?.AccountCode}.",
                        Duration = 5000
                    });

                    _hasUnsavedChanges = false;
                }
                else
                {
                    Logger!.LogError("Failed to delete gl account {GLACCOUNT}: {ERROR}.", _chartOfAccountDto?.AccountTitle, result.Error.Message);

                    DisplayErrorNotification
                    (
                        "Delete operation failed",
                        result.Error.Message,
                        true
                    );
                }

                Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while deleting gl account.");

            DisplayErrorNotification
            (
                "Delete operation failed",
                "An unexpected error occurred while performing gl account deletion.",
                true
            );

            Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
        }
    }

    private async Task Cancel()
    {
        if (_hasUnsavedChanges)
        {
            string msg = "There are unsaved changes. Leave without saving?";
            var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if ((bool)dialogResponse!)
            {
                Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
            }
        }
        else
        {
            Navigation?.NavigateTo("/Pages/Coa/CoaListPage");
        }
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (context.IsNavigationIntercepted && _hasUnsavedChanges)
        {
            var msg = "There are unsaved changes. Leave without saving?";

            var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (!(bool)dialogResponse!)
            {
                context.PreventNavigation();
            }
        }
    }

    private void DisplayErrorNotification(string title, string message, bool showAlert)
    {
        _errorAlertTitle = title;
        _errorAlertMessage = message;
        _showErrorAlert = showAlert;
    }

    private void OnAlertClose()
    {
        _showErrorAlert = false;
        _errorAlertMessage = string.Empty;
        _errorAlertTitle = string.Empty;
    }
}