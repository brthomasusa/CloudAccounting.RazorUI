using CloudAccounting.Wasm.Models.Coa;
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
    private bool _showErrorAlert;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool isLoading;




    private void OnAlertClose()
    {
        _showErrorAlert = false;
        _errorAlertMessage = string.Empty;
        _errorAlertTitle = string.Empty;
    }
}