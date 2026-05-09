
using CloudAccounting.Wasm.Models.Authentication;
using CloudAccounting.Wasm.Services.Repositories.Authentication;

namespace CloudAccounting.Wasm.Pages.Users
{
    public partial class UsersListPage
    {
        private bool _showErrorAlert;
        private bool _showUpdateComponent = true;
        private string _errorAlertMessage = string.Empty;
        private string _errorAlertTitle = string.Empty;
        private int _selectedCompanyCode;
        private List<RoleModel>? _roles;
        private RoleModel _newRoleModel = new();
        private CreateUserWithRoleCommand _newUserCommand = new();
        private UpdateUserRoleCommand? _updateUserRoleCommand;

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

            Result<List<ApplicationUser>> result = await AuthenticationService!.GetUsersByCompanyAndGroupAsync(_selectedCompanyCode, role!.GroupId);

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

        private async Task OnChange(TreeEventArgs args)
        {
            Logger!.LogInformation("Change{Args}", $"Item Text: {args.Text}");

            Result<ApplicationUser> result = await AuthenticationService!.GetUserByIdAsync(args.Text!);

            if (result.IsSuccess)
            {
                ApplicationUser user = result.Value;

                _updateUserRoleCommand = new UpdateUserRoleCommand
                {
                    Email = user.UserId,
                    RoleName = user.GroupTitle,
                    IsCompanyAdmin = user.GroupTitle == "CompanyAdmin"
                };
            }
            else
            {
                Logger!.LogError("Failed to retrieve user details: {Error}", result.Error.Message);
                _errorAlertTitle = "Error retrieving user details";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }

        private async Task OnCreateRole()
        {
            Result result = await AuthenticationService!.CreateRoleAsync(_newRoleModel);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(NotificationSeverity.Success, "Success", "Role created successfully.");
                _newRoleModel = new RoleModel(); // Reset form
                await OnInitializedAsync(); // Refresh roles list
            }
            else
            {
                Logger!.LogError("Failed to create role: {Error}", result.Error.Message);
                _errorAlertTitle = "Error creating role";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }

        private async Task OnCreateUserWithRole(CreateUserWithRoleCommand user)
        {            
            Result result = await AuthenticationService!.CreateUserWithRoleAsync(user);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(NotificationSeverity.Success, "Success", "User with role created successfully.");
                _newUserCommand = new CreateUserWithRoleCommand(); // Reset form
                _showUpdateComponent = true; // Show update component after creating user
                await OnInitializedAsync(); // Refresh roles list
            }
            else
            {
                Logger!.LogError("Failed to create user with role: {Error}", result.Error.Message);
                _errorAlertTitle = "Error creating user with role";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }

        private async Task OnUserUpdated(UpdateUserRoleCommand user)
        {
            Result result = await AuthenticationService!.UpdateUserRoleAsync(user);

            if (result.IsSuccess)
            {
                NotificationService!.Notify(NotificationSeverity.Success, "Success", "User role updated successfully.");
                _newUserCommand = new CreateUserWithRoleCommand(); // Reset form
                await OnInitializedAsync(); // Refresh roles list
            }
            else
            {
                Logger!.LogError("Failed to create user with role: {Error}", result.Error.Message);
                _errorAlertTitle = "Error creating user with role";
                _errorAlertMessage = result.Error.Message;
                _showErrorAlert = true;
            }
        }

        private void ShowCreateUserForm()
        {
            _showUpdateComponent = false;
        }

        private void OnAlertClose()
        {
            _showErrorAlert = false;
            _errorAlertMessage = string.Empty;
            _errorAlertTitle = string.Empty;
        }
    }
}