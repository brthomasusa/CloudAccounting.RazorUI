using CloudAccounting.Wasm.Models.CostCenters;
using CloudAccounting.Wasm.Services.Repositories.CostCenters;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.CostCenters;

public partial class CostCentersCreatePage
{
    [Parameter] public int CompanyCode { get; set; }
    [Inject] private ICostCenterService? CostCenterService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private DialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CostCentersCreatePage>? Logger { get; set; }

    private CreateCostCenterCommand _costCenter = new();
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _hasUnsavedChanges;

    protected override async Task OnInitializedAsync()
    {
        _costCenter.CompanyCode = CompanyCode;
        await base.OnInitializedAsync();
    }

    private async Task Submit(CreateCostCenterCommand arg)
    {
        try
        {
            var createResult = await CostCenterService!.CreateAsync(arg);

            if (createResult.IsSuccess)
            {
                var message = $"Cost center {arg.CostCenterTitle} created successfully.";

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Create operation succeeded",
                    Detail = message,
                    Duration = 5000
                });

                Navigation?.NavigateTo("/Pages/CostCenters/CostCentersListPage");
            }
            else
            {
                Logger!.LogError("Failed to create cost center: {ERROR}.", createResult.Error.Message);

                _errorAlertTitle = "Create operation failed";
                _errorAlertMessage = createResult.Error.Message;
                _showErrorAlert = true;
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while creating cost center.");

            _errorAlertTitle = "Create operation failed";
            _errorAlertMessage = "An unexpected error occurred while performing cost center creation.";
            _showErrorAlert = true;

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