using CloudAccounting.Wasm.Services.Repositories.Coa;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.Coa;

public partial class CreateCoaPage
{
    [Parameter] public int CompanyCode { get; set; }
    [Inject] private ICoaService? CoaService { get; set; }
    [Inject] private ILookupService? LookupService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private DialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CreateCoaPage>? Logger { get; set; }

    private CreateChartOfAccountCommand? _coaCommand;
    private List<CostCenterLookupItem>? _costCenterLookupItems;
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _hasUnsavedChanges;

    protected override async Task OnInitializedAsync()
    {
        var result = await LookupService!.GetCostCenterLookups(CompanyCode);
        _costCenterLookupItems = result.Value;

        _coaCommand = new ();
        _coaCommand.CompanyCode = CompanyCode;
        await base.OnInitializedAsync();
    }

    private async Task Submit(CreateChartOfAccountCommand arg)
    {
        try
        {
            var result = await CoaService!.CreateAsync(arg);

            if (result.IsSuccess)
            {
                var message = $"GL account {arg.AccountTitle} created successfully.";

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Create operation succeeded",
                    Detail = message,
                    Duration = 5000
                });

                _coaCommand = new CreateChartOfAccountCommand();
                _coaCommand.CompanyCode = arg.CompanyCode;
                _hasUnsavedChanges = false;
            }
            else
            {
                Logger!.LogError("Failed to create gl account: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Create operation failed";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while creating gl account.");

            _errorAlertTitle = "Create operation failed";
            _errorAlertMessage = "An unexpected error occurred while performing gl account creation.";
            _showErrorAlert = true;

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

    private static bool ValidateCostCenter(string costCenter)
    {
        return true;
    }

    private List<GeneralLedgerAccountType> _generalLedgerAccountTypes = new()
            {
        new() { Classification = "1", Name = "Equity" },
        new() { Classification = "2", Name = "Liability" },
        new() { Classification = "3", Name = "Asset" },
        new() { Classification = "4", Name = "Revenue" },
        new() { Classification = "5", Name = "Expense" }
    };
}

public class GeneralLedgerAccountType
{
    public string Classification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}