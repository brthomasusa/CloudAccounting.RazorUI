using CloudAccounting.Wasm.Models.VoucherTypes;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;

namespace CloudAccounting.Wasm.Pages.VoucherTypes
{
    public partial class VoucherTypesListPage
    {
        [Inject] private IVoucherTypeService? VoucherTypeService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<VoucherTypesListPage>? Logger { get; set; }

        private List<VoucherTypeDto>? _voucherTypes;
        private bool _showErrorAlert = false;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private bool isLoading;

        protected async override Task OnInitializedAsync()
        {
            try
            {
                Result<List<VoucherTypeDto>> result = await VoucherTypeService!.RetrieveAllAsync();

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve voucher types: {ERROR}.", result.Error.Message);

                    _errorAlertTitle = "Error retrieving voucher types";
                    _errorAlertMessage = result.Error.Message;
                    _showErrorAlert = true;

                    Navigation?.NavigateTo("/");
                }

                _voucherTypes = result.Value;

                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving voucher types.");

                _errorAlertTitle = "Error retrieving voucher types";
                _errorAlertMessage = "An unexpected error occurred while retrieving voucher types.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }
        }

        private async Task GetVoucherTypes(LoadDataArgs args)
        {
            try
            {
                Result<List<VoucherTypeDto>> result = await VoucherTypeService!.RetrieveAllAsync();

                if (result.IsFailure)
                {
                    Logger!.LogError("Failed to retrieve voucher types: {ERROR}.", result.Error.Message);

                    _errorAlertTitle = "Error retrieving voucher types";
                    _errorAlertMessage = result.Error.Message;
                    _showErrorAlert = true;

                    Navigation?.NavigateTo("/");
                }

                isLoading = true;
                _voucherTypes = result.Value;
                isLoading = false;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while retrieving voucher types.");

                _errorAlertTitle = "Error retrieving voucher types";
                _errorAlertMessage = "An unexpected error occurred while retrieving voucher types.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/");
            }
        }

        private void ViewVoucherTypeDetails(VoucherTypeDto model)
        {
            Navigation!.NavigateTo($"/Pages/VoucherTypes/VoucherTypesEditPage/{model.VoucherCode}");
        }

        private void GoToCreateVoucherTypePage()
        {
            Navigation!.NavigateTo("/Pages/VoucherTypes/VoucherTypesCreatePage");
        }

        private void OnAlertClose()
        {
            _showErrorAlert = false;
            _errorAlertMessage = string.Empty;
            _errorAlertTitle = string.Empty;
        }
    }
}
