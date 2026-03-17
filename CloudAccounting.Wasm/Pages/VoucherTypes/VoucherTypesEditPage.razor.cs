using CloudAccounting.Wasm.Models.VoucherTypes;
using CloudAccounting.Wasm.Services.Repositories.VoucherTypes;
using Microsoft.AspNetCore.Components.Routing;

namespace CloudAccounting.Wasm.Pages.VoucherTypes
{
    public partial class VoucherTypesEditPage
    {
        [Parameter] public int VoucherCode { get; set; }
        [Inject] private IVoucherTypeService? VoucherService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private DialogService? DialogService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private ILogger<VoucherTypesEditPage>? Logger { get; set; }

        private VoucherTypeDto? _voucherType;
        private readonly List<VoucherTypeClassification> _voucherTypeClassifications = VoucherTypeClassificationList.Classifications;
        private string? _voucherTitle;
        private bool _showErrorAlert = false;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private bool _hasUnsavedChanges = false;

        //Enum.GetValues<VoucherTypeClassification>().ToList();

        protected override async Task OnParametersSetAsync()
        {
            Result<VoucherTypeDto> result = await VoucherService!.RetrieveAsync(VoucherCode);

            if (result.IsSuccess)
            {
                _voucherType = result.Value;
                _voucherTitle = result.Value.VoucherTitle;
            }
            else
            {
                Logger!.LogError("Failed to retrieve voucher type: {ERROR}.", result.Error.Message);

                _errorAlertTitle = "Error retrieving voucher type";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;

                Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
            }
        }

        private async Task Submit(VoucherTypeDto arg)
        {
            try
            {
                VoucherTypeCommand command = new () 
                { 
                    VoucherCode = arg.VoucherCode, 
                    VoucherType = arg.VoucherType, 
                    VoucherTitle = arg.VoucherTitle, 
                    VoucherClassification = arg.VoucherClassification
                };

                Result updateResult = await VoucherService!.UpdateAsync(command);

                if (updateResult.IsSuccess)
                {
                    string message = $"Voucher type {arg.VoucherType} updated successfully.";

                    NotificationService!.Notify(new NotificationMessage
                    {
                        Style = "position: absolute; inset-inline-start: -1000px;",
                        Severity = NotificationSeverity.Success,
                        Summary = "Update operation succeeded",
                        Detail = message,
                        Duration = 5000
                    });


                    Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
                }
                else
                {
                    Logger!.LogError("Failed to update voucher type: {ERROR}.", updateResult.Error.Message);

                    _errorAlertTitle = "Update operation failed";
                    _errorAlertMessage = updateResult.Error.Message;
                    _showErrorAlert = true;
                }
            }
            catch (Exception ex) 
            {
                Logger!.LogError(ex, "An exception occurred while updating voucher type.");

                _errorAlertTitle = "Update operation failed";
                _errorAlertMessage = "An unexpected error occurred while performing voucher type update.";
                _showErrorAlert = true;

                Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
            }
        }

        private async Task Delete()
        {
            try
            {
                string msg = $"Do you wish to delete voucher type {_voucherTitle}? This can't be undone!";
                var dialogResponse = await DialogService!.Confirm(msg, $"Delete {_voucherTitle}?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

                if ((bool)dialogResponse)
                {
                    Result result = await VoucherService!.DeleteAsync(_voucherType!.VoucherCode);

                    if (result.IsSuccess)
                    {
                        NotificationService!.Notify(new NotificationMessage
                        {
                            Style = "position: absolute; inset-inline-start: -1000px;",
                            Severity = NotificationSeverity.Success,
                            Summary = "Delete succeeded",
                            Detail = $"Successfully deleted voucher type {_voucherTitle}.",
                            Duration = 5000
                        });

                        _hasUnsavedChanges = false;
                    }
                    else
                    {
                        Logger!.LogError("Failed to delete voucher type {VOUCHER}: {ERROR}.", _voucherTitle, result.Error.Message);

                        _errorAlertTitle = "Update operation failed";
                        _errorAlertMessage = result.Error.Message;
                        _showErrorAlert = true;
                    }

                    Navigation?.NavigateTo("/Pages/VoucherTypes/VoucherTypesListPage");
                }
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "An exception occurred while updating voucher type.");

                _errorAlertTitle = "Update operation failed";
                _errorAlertMessage = "An unexpected error occurred while performing voucher type update.";
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

                if ((bool)dialogResponse)
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
            return voucherClassification > 0;
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (context.IsNavigationIntercepted && _hasUnsavedChanges)
            {
                string msg = "There are unsaved changes. Leave without saving?";
                var dialogResponse = await DialogService!.Confirm(msg, "Leave without saving?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

                if (!(bool)dialogResponse)
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
