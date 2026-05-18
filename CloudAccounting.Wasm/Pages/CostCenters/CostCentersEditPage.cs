
using CloudAccounting.Wasm.Models.CostCenters;
using CloudAccounting.Wasm.Services.Repositories.CostCenters;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.CostCenters;

public partial class CostCentersEditPage
{
    [Parameter] public int CompanyCode { get; set; }
    [Parameter] public string? CostCenterCode { get; set; }
    [Inject] private ICostCenterService? CostCenterService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private DialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CostCentersEditPage>? Logger { get; set; }

    private CostCenterDto? _costCenter;
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _hasUnsavedChanges;
    
    protected override async Task OnInitializedAsync()
    {
        var result = await CostCenterService!.RetrieveByIdAsync(CompanyCode, CostCenterCode!);

        if (result.IsSuccess)
        {
            _costCenter = result.Value;
        }
        else
        {
            Logger?.LogError("Failed to retrieve cost center: {Error}", result.Error);
            NotificationService?.Notify(NotificationSeverity.Error, "Error", "Failed to retrieve cost center.");
        }
    }

    private async Task Submit(CostCenterDto arg)
    {
        try
        {
            UpdateCostCenterCommand command = new(arg.CompanyCode, arg.CostCenterCode, arg.CostCenterTitle!);            

            var updateResult = await CostCenterService!.UpdateAsync(command);

            if (updateResult.IsSuccess)
            {
                var message = $"Cost center {arg.CostCenterTitle} updated successfully.";

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Update operation succeeded",
                    Detail = message,
                    Duration = 5000
                });

                Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
            }
            else
            {
                Logger!.LogError("Failed to update cost center: {ERROR}.", updateResult.Error.Message);

                _errorAlertTitle = "Update operation failed";
                _errorAlertMessage = updateResult.Error.Message;
                _showErrorAlert = true;
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while updating cost center.");

            _errorAlertTitle = "Update operation failed";
            _errorAlertMessage = "An unexpected error occurred while performing cost center update.";
            _showErrorAlert = true;

            Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
        }
    }

    private async Task Delete()
    {
        try
        {
            string msg = $"Do you wish to delete cost center {_costCenter?.CostCenterTitle}? This can't be undone!";
            var dialogResponse = await DialogService!.Confirm(msg, $"Delete {_costCenter?.CostCenterTitle}?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if ((bool)dialogResponse!)
            {
                Result result = await CostCenterService!.DeleteAsync(_costCenter!.CompanyCode, _costCenter.CostCenterCode);

                if (result.IsSuccess)
                {
                    NotificationService!.Notify(new NotificationMessage
                    {
                        Style = "position: absolute; inset-inline-start: -1000px;",
                        Severity = NotificationSeverity.Success,
                        Summary = "Delete succeeded",
                        Detail = $"Successfully deleted cost center {_costCenter?.CostCenterTitle}.",
                        Duration = 5000
                    });

                    _hasUnsavedChanges = false;
                }
                else
                {
                    Logger!.LogError("Failed to delete cost center {COSTCENTER}: {ERROR}.", _costCenter?.CostCenterTitle, result.Error.Message);

                    DisplayErrorNotification
                    (
                        "Update operation failed",
                        result.Error.Message,
                        true
                    );
                }

                Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while deleting cost center.");

            DisplayErrorNotification
            (
                "Update operation failed", 
                "An unexpected error occurred while performing cost center deletion.", 
                true
            );

            Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
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
                Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
            }
        }
        else
        {
            Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
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