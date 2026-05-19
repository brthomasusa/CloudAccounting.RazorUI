namespace CloudAccounting.Wasm.Pages.CostCenters;

using CloudAccounting.Wasm.Models.CostCenters;
using CloudAccounting.Wasm.Services.Repositories.CostCenters;

public partial class CostCentersListPage
{
    [Inject] private ICostCenterService? CostCenterService { get; set; }
    [Inject] private NotificationService? NotificationService { get; set; }
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ILogger<CostCentersListPage>? Logger { get; set; }
    [Inject] private IAuthenticatedUserState? AuthenticatedUserState { get; set; }

    private List<CostCenterDto>? _costCenters;
    private int _selectedCompanyCode;
    private bool _showErrorAlert = false;
    private string _errorAlertMessage = string.Empty;
    private string _errorAlertTitle = string.Empty;
    private bool isLoading;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _selectedCompanyCode = AuthenticatedUserState!.GetUser().CompanyCode;

            var result = await CostCenterService!.RetrieveAllAsync(_selectedCompanyCode);

            if (result.IsFailure)
            {
                Logger!.LogError("Failed to retrieve cost centers: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Error retrieving cost centers";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            _costCenters = result.Value;

            await base.OnInitializedAsync();
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

    private async Task GetCostCenters(LoadDataArgs args)
    {
        try
        {
            var result = await CostCenterService!.RetrieveAllAsync(_selectedCompanyCode);

            if (result.IsFailure)
            {
                Logger!.LogError("Failed to retrieve cost centers: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Error retrieving cost centers";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }

            isLoading = true;
            _costCenters = result.Value;
            isLoading = false;
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

    private void ViewCostCenterDetails(CostCenterDto model)
    {
        Navigation!.NavigateTo($"/Pages/CostCenters/CostCentersEditPage/{model.CompanyCode}/{model.CostCenterCode}");
    }

    private void GoToCreateCostCenterPage()
    {
        Navigation!.NavigateTo($"/Pages/CostCenters/CostCentersCreatePage/{_selectedCompanyCode}");
    }



    private void OnAlertClose()
    {
        _showErrorAlert = false;
        _errorAlertMessage = string.Empty;
        _errorAlertTitle = string.Empty;
    }
}