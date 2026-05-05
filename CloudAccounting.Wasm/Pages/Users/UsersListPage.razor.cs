
using CloudAccounting.Wasm.Models.Authentication;
using CloudAccounting.Wasm.Services.Repositories.Authentication;

namespace CloudAccounting.Wasm.Pages.Users
{
    public partial class UsersListPage
    {
        private bool _showErrorAlert;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private int _selectedCompanyCode;
        private List<RoleModel>? _roles;

        [Inject] public DialogService? DialogService { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private NavigationManager? Navigation { get; set; }
        [Inject] private IAuthenticatedUserState? AuthenticatedUserState { get; set; }
        [Inject] private IAuthenticationService? AuthenticationService { get; set; }
        [Inject] private ILogger<UsersListPage>? Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _selectedCompanyCode = AuthenticatedUserState!.GetUser().CompanyCode;

                Result<List<RoleModel>> rolesResult = await AuthenticationService!.GetAllRolesAsync();

                if (rolesResult.IsSuccess)
                {
                    _roles = rolesResult.Value;
                }
                else
                {
                    Logger!.LogError("Failed to retrieve roles: {Error}", rolesResult.Error.Message);
                }

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

        private async Task OnExpand(TreeExpandEventArgs args)
        {
            var role = args.Value as RoleModel;

            Result<List<ApplicationUser>> result = await AuthenticationService!.LoadUsersByCompanyAndGroupAsync(_selectedCompanyCode, role!.GroupId);

            args.Children!.Data = result.IsSuccess ? result.Value : [];
            args.Children.TextProperty = "UserId";
            // args.Children.Selected = context => (context.Value as ApplicationUser)?.UserId == AuthenticatedUserState!.GetUser().UserId;

            // Optional template
            args.Children.Template = context => builder => {
                builder.OpenElement(1, "strong");
                builder.AddContent(2, (context.Value as ApplicationUser)?.UserId);
                builder.CloseElement();
            };            
        }

        private void OnChange(TreeEventArgs args)
        {
            Logger!.LogInformation("Change{Args}", $"Item Text: {args.Text}");
        }

        private void OnAlertClose()
        {
            _showErrorAlert = false;
            _errorAlertMessage = string.Empty;
            _errorAlertTitle = string.Empty;
        }
    }
}