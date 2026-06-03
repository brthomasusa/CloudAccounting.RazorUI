using CloudAccounting.Wasm.Models.VoucherTypes;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.VoucherTypes
{
    public partial class VoucherTypesCreatePage
    {
        [Inject] private IVoucherTypeService? VoucherService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private DialogService? DialogService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<VoucherTypesCreatePage>? Logger { get; set; }

        private VoucherTypeCommand? _voucherTypeCmd;
        private readonly List<VoucherTypeClassification> _voucherTypeClassifications = VoucherTypeClassificationList.Classifications;

        private bool _showErrorAlert;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private bool _hasUnsavedChanges;

        protected override async Task OnInitializedAsync()
        {
            _voucherTypeCmd = new VoucherTypeCommand() 
            { 
                VoucherType = string.Empty,
                VoucherTitle = string.Empty,
                VoucherClassification = 0
            };

            await base.OnInitializedAsync();
        }

        private async Task Submit(VoucherTypeCommand arg)
        {
            try
            {
                Result result = await VoucherService!.CreateAsync(arg);

                if (result.IsSuccess)
                {
                    string message = $"Voucher type {arg.VoucherType} created successfully.";

                    NotificationService!.Notify(new NotificationMessage
                    {
                        Style = "position: absolute; inset-inline-start: -1000px;",
                        Severity = NotificationSeverity.Success,
                        Summary = "Create operation succeeded",
                        Detail = message,
                        Duration = 5000
                    });


                    Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
                }
                else
                {
                    Logger!.LogError("Failed to create voucher type: {ERROR}.", result.Error.Message);

                    _errorAlertTitle = "Update operation failed";
                    _errorAlertMessage = result.Error.Message;
                    _showErrorAlert = true;
                }
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while creating voucher type.");

                _errorAlertTitle = "Create operation failed";
                _errorAlertMessage = "An unexpected error occurred while performing voucher type creation.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
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
                    Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
                }
            }
            else
            {
                Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
            }
        }

        private static bool ValidateVoucherClassification(byte voucherClassification)
        {
            return voucherClassification > 0 && voucherClassification < 4;
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (context.IsNavigationIntercepted && _hasUnsavedChanges)
            {
                string msg = "There are unsaved changes. Leave without saving?";
                var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

                if (!(bool)dialogResponse!)
                {
                    context.PreventNavigation();
                }
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
