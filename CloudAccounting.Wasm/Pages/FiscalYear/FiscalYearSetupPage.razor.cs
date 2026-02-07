using CloudAccounting.Wasm.Services.Repositories.Company;
using Radzen.Blazor;

namespace CloudAccounting.Wasm.Pages.FiscalYear
{
    public partial class FiscalYearSetupPage
    {
        [Inject] private ILookupService? LookupService { get; set; }
        [Inject] private ICompanyService? CompanyService { get; set; }
        [Inject] public DialogService? DialogService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<FiscalYearSetupPage>? Logger { get; set; }

        //private RadzenTemplateForm<CreateFiscalYearCommand>? _fiscalYearParametersForm;
        private CreateFiscalYearCommand _fiscalYearCommand = new(0,0,0);
        private List<CompanyLookup>? _companyLookups;
        private List<MonthLookup> _months = MonthLookupList.Months;
        private CompanyWithFiscalPeriodsDto _companyWithFiscalPeriods = new();
        private int _selectedCompanyCode;
        private DateTime _validNextFiscalYearStartDate = DateTime.MinValue;

        private bool isLoading;

        protected async override Task OnInitializedAsync()
        {
            try
            {
                Result<List<CompanyLookup>> result = await LookupService!.GetCompanyLookups();

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve company lookups: {ERROR}.", result.Error.Message);

                    ShowErrorNotification.ShowError(
                        NotificationService!,
                        result.Error.Message
                    );

                    Navigation?.NavigateTo("/");
                }

                _companyLookups = result.Value;
                CompanyLookup unselectedItem = new(0, "------");
                _companyLookups.Insert(0, unselectedItem);

                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving company lookups.");
                ShowErrorNotification.ShowError(
                    NotificationService!,
                    "An unexpected error occurred while retrieving company lookups."
                );

                Navigation?.NavigateTo("/");
            }
        }

        private static bool ValidateCompanyCode(int companyCode)
        {
            return companyCode > 0;
        }

        private bool ValidateFiscalYearWhenPreviousFiscalYear(int fiscalYear)
        {
            if (fiscalYear >= _validNextFiscalYearStartDate.Year || fiscalYear < DateTime.Now.AddMonths(11).Year)
            {
                return true;
            }

            return false;
        }

        private bool ValidateFiscalYearWhenNoPreviousFiscalYear(int fiscalYear)
        {
            if (_validNextFiscalYearStartDate == DateTime.MinValue)
            {
                if (fiscalYear < 2000 || fiscalYear > DateTime.Now.AddMonths(11).Year)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateStartMonth(int monthId)
        {
            return monthId > 0;
        }

        private async Task OnCompanyDropDownChanged(object companyCode)
        {
            try
            {
                _selectedCompanyCode = Convert.ToInt32(companyCode);

                Result<DateTime> validStartDateresult = await CompanyService!.GetNextValidFiscalYearStartDateAsync(_selectedCompanyCode);

                if (validStartDateresult.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve next valid fiscal year start date: {ERROR}.", validStartDateresult.Error.Message);

                    ShowErrorNotification.ShowError(
                        NotificationService!,
                        validStartDateresult.Error.Message
                    );

                    Navigation?.NavigateTo("/");
                }

                _validNextFiscalYearStartDate = validStartDateresult.Value;

                Result<CompanyWithFiscalPeriodsDto> result = await CompanyService!.GetCompanyFiscalYearAsync(_selectedCompanyCode);

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve company fiscal year info: {ERROR}.", result.Error.Message);

                    ShowErrorNotification.ShowError(
                        NotificationService!,
                        result.Error.Message
                    );

                    Navigation?.NavigateTo("/");
                }

                _companyWithFiscalPeriods = result.Value;
            }
            catch (Exception ex) 
            {
                Logger!.LogError(ex, "An exception occurred while retrieving company lookups.");
                ShowErrorNotification.ShowError(
                    NotificationService!,
                    "An unexpected error occurred while retrieving company lookups."
                );

                Navigation?.NavigateTo("/");
            }
        }

        private async Task GetFiscalPeriods(LoadDataArgs args)
        {
            try
            {
                Result<CompanyWithFiscalPeriodsDto> result = await CompanyService!.GetCompanyFiscalYearAsync(_selectedCompanyCode);

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve company fiscal year info: {ERROR}.", result.Error.Message);

                    ShowErrorNotification.ShowError(
                        NotificationService!,
                        result.Error.Message
                    );

                    Navigation?.NavigateTo("/");
                }

                isLoading = true;
                _companyWithFiscalPeriods = result.Value;
                isLoading = false;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving company fiscal year info.");
                ShowErrorNotification.ShowError(
                    NotificationService!,
                    "An unexpected error occurred while retrieving company fiscal year info."
                );

                Navigation?.NavigateTo("/");
            }
        }

        private async Task Submit(CreateFiscalYearCommand arg)
        {
            //CreateFiscalYearCommand command = new(arg.CompanyCode, arg.FiscalYear, arg.StartMonthNumber);

            Result<CompanyWithFiscalPeriodsDto> result = await CompanyService!.CreateCompanyFiscalYearAsync(_fiscalYearCommand);

            if (result.IsSuccess)
            {
                _fiscalYearCommand = new(0, 0, 0);

                NotificationService!.Notify(new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Success,
                    Summary = "New company created",
                    Detail = $"Successfully created new fiscal year for {result.Value.CompanyName}.",
                    Duration = 4000
                });
                
                _companyWithFiscalPeriods = result.Value;                

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                Logger!.LogError("Failed to create fiscal year: {ERROR}.", result.Error.Message);

                ShowErrorNotification.ShowError(
                    NotificationService!,
                    result.Error.Message
                );
            }
        }

    }
}
