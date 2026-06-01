
using CloudAccounting.Wasm.Services.Repositories.Coa;

namespace CloudAccounting.Wasm.Pages.Coa;

public partial class CoaListPage
{
    [Inject] private ICoaService? CoaService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CoaListPage>? Logger { get; set; }
    [Inject] private IAuthenticatedUserState? AuthenticatedUserState { get; set; }

    private PagedResponse<ChartOfAccountDto>? _chartOfAccounts;
    private int _selectedCompanyCode;
    private readonly IEnumerable<int> pageSizeOptions = [5, 10, 15, 20];
    private int _totalRecords;
    private string _accountFilter = string.Empty;
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _selectedCompanyCode = AuthenticatedUserState!.GetUser().CompanyCode;

            var result = await CoaService!.RetrieveAllAsync(1, 15, _selectedCompanyCode);

            if (result.IsFailure)
            {
                Logger!.LogError("Failed to retrieve chart of accounts: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Error retrieving chart of accounts";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _chartOfAccounts = result.Value;
            _totalRecords = _chartOfAccounts.TotalRecords;

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

    private async Task GetChartOfAccounts(LoadDataArgs args)
    {
        try
        {
            //Logger!.LogInformation("CoaListPage.GetChartOfAccounts: {DATA}.", args.ToJson());

            if (args.Filters is not null)
            {
                List<FilterDescriptor> descriptors = args.Filters.ToList();

                FilterDescriptor? filterDescriptor
                    = descriptors.Find(x => !string.IsNullOrEmpty(x.Property) && !string.IsNullOrEmpty(x.FilterValue?.ToString()));

                if (filterDescriptor is not null)
                {
                    _accountFilter = filterDescriptor.FilterValue!.ToString()!;
                }
                else
                {
                    _accountFilter = string.Empty;
                }
            }
            else
            {
                _accountFilter = string.Empty;
            }

            Result<PagedResponse<ChartOfAccountDto>> result;
            int pageNumber = ((args.Skip ?? 0) / (args.Top ?? 15)) + 1;
            int pageSize = args.Top ?? 15;

            if (!string.IsNullOrEmpty(_accountFilter))
            {
                result = await CoaService!.RetrieveAllAsync(pageNumber, pageSize, _selectedCompanyCode, _accountFilter);
            }
            else
            {
                result = await CoaService!.RetrieveAllAsync(pageNumber, pageSize, _selectedCompanyCode);
            }

            if (result.IsFailure)
            {
                Logger!.LogError("Failed to retrieve chart of accounts: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Error retrieving chart of accounts";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _isLoading = true;

            _chartOfAccounts = result.Value;
            _totalRecords = _chartOfAccounts.TotalRecords;

            _isLoading = false;
            //Logger!.LogInformation("CoaListPage.GetChartOfAccounts: {DATA}.", _chartOfAccounts.Data.ToJson());
            await InvokeAsync(StateHasChanged);

        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "An exception occurred while retrieving cost centers.");

            _errorAlertTitle = "Error retrieving cost centers";
            _errorAlertMessage = "An unexpected error occurred while retrieving cost centers.";
            _showErrorAlert = true;

            Navigation?.NavigateTo("/");
        }
    }

    private void GoToCreateCoaPage()
    {
        Navigation!.NavigateTo($"/Pages/Coa/CoaCreatePage/{_selectedCompanyCode}");
    }

    private void ViewCoaDetails(ChartOfAccountDto model)
    {
        Navigation!.NavigateTo($"/Pages/Coa/CoaEditPage/{model.CompanyCode}/{model.AccountCode}");
    }

    private void OnAlertClose()
    {
        _showErrorAlert = false;
        _errorAlertMessage = string.Empty;
        _errorAlertTitle = string.Empty;
    }
}