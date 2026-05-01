using CloudAccounting.Wasm.Services.Repositories.Company;

namespace CloudAccounting.Wasm.Pages.FiscalYear
{
    public partial class FiscalYearSetupPage
    {

        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] public DialogService? DialogService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<FiscalYearSetupPage>? Logger { get; set; }
        [Inject] private IAuthenticatedUserState? AuthenticatedUserState { get; set; }

        private CreateFiscalYearCommand _fiscalYearCommand = new(0,0,DateTime.MinValue);
        private FiscalYearDto? _fiscalYearDto = null;
        private int _selectedCompanyCode;
        private bool _disableFiscalYearDeleteButton = true;
        private bool _disableGenerateFiscalYearButton = true;
        private bool _showErrorAlert = false;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private bool _isLoading = false;

        protected async override Task OnInitializedAsync()
        {
            try
            {
                int companyCode = AuthenticatedUserState!.GetUser().CompanyCode;
                _fiscalYearCommand.CompanyCode = companyCode;

                await GetCurrentFiscalYear(companyCode);

                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving company lookups.");
                _errorAlertTitle = "Error retrieving company lookup data";
                _errorAlertMessage = "An unexpected error occurred while retrieving company lookups.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }
        }

        private async Task GetCurrentFiscalYear(int companyCode)
        {
            if (companyCode == 0) 
            { 
                _disableGenerateFiscalYearButton = true;
            } 
            else 
            {
                await GetFiscalYears(companyCode);
            }            
        }


        private async Task GetFiscalYears(int companyCode)
        {
            try
            {
                _selectedCompanyCode = companyCode;

                Result<FiscalYearDto> result = await CompanyService!.GetCompanyFiscalYearAsync(_selectedCompanyCode);

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve company fiscal year info: {ERROR}.", result.Error.Message);
                    _errorAlertTitle = "Error retrieving fiscal year information";
                    _errorAlertMessage = result.Error.Message;
                    _showErrorAlert = true;

                    Navigation?.NavigateTo("/");
                }

                _fiscalYearDto = result.Value;

                _fiscalYearCommand = new(
                        _fiscalYearDto.CompanyCode,
                        _fiscalYearDto.FiscalPeriods.Count == 0 ? DateTime.Today.Year : _fiscalYearDto.Year + 1,
                        _fiscalYearDto.FiscalPeriods.Count == 0 ? DateTime.Today : _fiscalYearDto.FiscalYearEndDate.AddDays(1)    
                    );

                _disableFiscalYearDeleteButton = _fiscalYearDto.HasTransactions || _fiscalYearDto.FiscalPeriods.Count == 0;
                _disableGenerateFiscalYearButton = false;

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving fiscal year information.");
                _errorAlertTitle = "Error retrieving fiscal year information.";
                _errorAlertMessage = "An unexpected error occurred while retrieving fiscal year information.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }
        }

        private async Task GetFiscalPeriods(LoadDataArgs args)
        {
            _isLoading = true;
            await GetFiscalYears(Convert.ToInt32(_selectedCompanyCode));
            _isLoading = false;
        }

        private async Task DeleteFiscalYear()
        {

            int companyCode = _fiscalYearDto!.CompanyCode;
            int fiscalYear = _fiscalYearDto.Year;
            string companyName = _fiscalYearDto.CompanyName!;

            string msg = $"Do you wish to delete fiscal year {fiscalYear} for {companyName}? This can't be undone!";

            var dialogResponse = await DialogService!.Confirm(msg, $"Delete fiscal year?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if ((bool)dialogResponse)
            {
                Result result = 
                    await CompanyService!.DeleteCompanyFiscalYearAsync(companyCode, fiscalYear);

                if (result.IsSuccess)
                {
                    string successMsg = $"Successfully deleted fiscal year {fiscalYear} for {companyName}.";
                    
                    NotificationService!.Notify(new NotificationMessage
                    {
                        Style = "position: absolute; inset-inline-start: -1000px;",
                        Severity = NotificationSeverity.Success,
                        Summary = "Delete succeeded",
                        Detail = successMsg,
                        Duration = 5000
                    });

                    await GetFiscalYears(_selectedCompanyCode);
                }
                else
                {
                    Logger!.LogError("Failed to delete company fiscal year: {ERROR}.", result.Error.Message);

                    _errorAlertTitle = "Error deleting fiscal year information";
                    _errorAlertMessage = "An unexpected error occurred while deleting fiscal year information.";
                    _showErrorAlert = true;
                }
            }
        }

        private async Task Submit(CreateFiscalYearCommand arg)
        {
            Result<FiscalYearDto> result = await CompanyService!.CreateCompanyFiscalYearAsync(arg);

            if (result.IsSuccess)
            {
                _fiscalYearCommand = new(0, 0, DateTime.MinValue);
                _disableGenerateFiscalYearButton = true;

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "New fiscal year created",
                    Detail = $"Successfully created fiscal year {result.Value.Year} for {result.Value.CompanyName}.",
                    Duration = 5000
                });
                
                _fiscalYearDto = result.Value;
                _disableFiscalYearDeleteButton = _fiscalYearDto.FiscalPeriods.Count == 0;

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                Logger!.LogError("Failed to create fiscal year: {ERROR}.", result.Error.Message);
                _errorAlertTitle = "Error creating fiscal year";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }

        private static bool ValidateCompanyCode(int companyCode)
        {
            return companyCode > 0;
        }

        private bool ValidateFiscalYear(int fiscalYear)
        {
            if (_fiscalYearDto!.FiscalPeriods.Count > 0)
            {
                return fiscalYear > _fiscalYearDto.Year;
            }

            return true;
        }

        private bool ValidateStartDate(DateTime startDate)
        {
            if (_fiscalYearDto!.FiscalPeriods.Count > 0)
            {
                return startDate > _fiscalYearDto.FiscalYearEndDate;
            }
            else
            {
                return startDate > DateTime.MinValue;
            }
        }

        private void OnAlertClose()
        {
            _showErrorAlert = false;
            _errorAlertMessage = string.Empty;
            _errorAlertTitle = string.Empty;
        }
    }
}
