
using CloudAccounting.Wasm.Models.Authentication;
using CloudAccounting.Wasm.Services.Repositories.FiscalPeriod;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.FiscalYear;

public partial class SetCurrentFiscalPeriod
{
    [Inject] private ILookupService? LookupService { get; set; }
    [Inject] private IFiscalPeriodService? FiscalPeriodService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private DialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<SetCurrentFiscalPeriod>? Logger { get; set; }
    [Inject] private IAuthenticatedUserState? AuthenticatedUserState { get; set; }

    private int _selectedCompanyCode;
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _hasUnsavedChanges;
    private List<FiscalYearLookupItem> _fiscalYears = [];
    private List<FiscalPeriodLookupItem> _fiscalPeriods = [];
    private UpdateUserFiscalPeriodCommand? _updateUserFiscalPeriodCommand;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _selectedCompanyCode = AuthenticatedUserState!.GetUser().CompanyCode;
            short selectedYear = AuthenticatedUserState.GetUser().CompanyYear;
            byte selectedMonthId = AuthenticatedUserState.GetUser().CompanyMonthId;
            
            var fiscalYearResult = await LookupService!.RetrieveFiscalYearsAsync(_selectedCompanyCode);            

            if (fiscalYearResult.IsFailure)
            {
                Logger!.LogError("Failed to retrieve chart of accounts: {ERROR}.", fiscalYearResult.Error.Message);

                _errorAlertTitle = "Error retrieving chart of accounts";
                _errorAlertMessage = fiscalYearResult.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _fiscalYears = fiscalYearResult.Value;

            var fiscalPeriodResult = await LookupService!.RetrieveFiscalPeriodsAsync(_selectedCompanyCode, selectedYear);

            if (fiscalPeriodResult.IsFailure)
            {
                Logger!.LogError("Failed to retrieve chart of accounts: {ERROR}.", fiscalPeriodResult.Error.Message);

                _errorAlertTitle = "Error retrieving chart of accounts";
                _errorAlertMessage = fiscalPeriodResult.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _fiscalPeriods = fiscalPeriodResult.Value;

            _updateUserFiscalPeriodCommand = new UpdateUserFiscalPeriodCommand(_selectedCompanyCode, selectedYear, selectedMonthId);
            Logger!.LogInformation("Initialized SetCurrentFiscalPeriod component with CompanyCode: {CompanyCode}, Year: {Year}, MonthId: {MonthId}.", _selectedCompanyCode, selectedYear, selectedMonthId);

            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while retrieving chart of accounts.");

            _errorAlertTitle = "Error retrieving chart of accounts";
            _errorAlertMessage = "An unexpected error occurred while retrieving chart of accounts.";
            _showErrorAlert = true;

            Navigation?.NavigateTo("/");
        }
    }

    private async Task Submit(UpdateUserFiscalPeriodCommand arg)
    {
        try
        {
            var updateResult = await FiscalPeriodService!.UpdateCurrentFiscalPeriodAsync(arg);

            if (updateResult.IsSuccess)
            {
                var message = "Fiscal period updated successfully.";

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "Update operation succeeded",
                    Detail = message,
                    Duration = 5000
                });
                
                string companyMonthName = _fiscalPeriods.FirstOrDefault(fp => fp.CompanyMonthId == arg.CompanyMonthId)?.CompanyMonthName ?? string.Empty;
                AuthenticatedUserState!.SetCompanyYear(arg.CompanyYear);
                AuthenticatedUserState!.SetCompanyMonth(arg.CompanyMonthId, companyMonthName);

                _hasUnsavedChanges = false;

                await InvokeAsync(StateHasChanged);

                Navigation?.NavigateTo("/");
            }
            else
            {
                Logger!.LogError("Failed to update fiscal period: {ERROR}.", updateResult.Error.Message);

                _errorAlertTitle = "Update operation failed";
                _errorAlertMessage = updateResult.Error.Message;
                _showErrorAlert = true;
            }
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while updating fiscal period.");

            _errorAlertTitle = "Update operation failed";
            _errorAlertMessage = "An unexpected error occurred while performing fiscal period update.";
            _showErrorAlert = true;

            Navigation?.NavigateTo("/Pages/FiscalYear/SetCurrentFiscalPeriod");
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
            Navigation?.NavigateTo("/");
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

    private async Task OnFiscalYearChanged(object args)
    {
        _hasUnsavedChanges = true;

        if ((short)args > 0)
        {
            var selectedFiscalYear = (short)args;

            _fiscalPeriods.Clear();

            var fiscalPeriodResult = await LookupService!.RetrieveFiscalPeriodsAsync(_selectedCompanyCode, selectedFiscalYear);

            if (fiscalPeriodResult.IsFailure)
            {
                Logger!.LogError("Failed to retrieve chart of accounts: {ERROR}.", fiscalPeriodResult.Error.Message);

                _errorAlertTitle = "Error retrieving chart of accounts";
                _errorAlertMessage = fiscalPeriodResult.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _fiscalPeriods.AddRange(fiscalPeriodResult.Value);
        }        
    }

    private void OnAlertClose()
    {
        _showErrorAlert = false;
        _errorAlertMessage = string.Empty;
        _errorAlertTitle = string.Empty;
    }
}