using Radzen;

namespace CloudAccounting.Wasm.Utilities
{
    public static class ShowErrorNotification
    {
        public static void ShowError
        (
            NotificationService notificationService,
            string errorMessage)
        {
            notificationService!.Notify(
                new NotificationMessage
                {
                    Style = "position: absolute; inset-inline-start: -1000px;",
                    Severity = NotificationSeverity.Error,
                    Summary = "The following error occured:",
                    Detail = errorMessage,
                    Duration = 40000
                }
            );
        }
    }
}
