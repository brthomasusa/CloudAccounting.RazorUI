using CloudAccounting.Wasm.Models.Authentication;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponseModel>> LoginAsync(LoginCommand request);

        Task<Result<string>> RefreshAuthTokenAsync();

        Task<Result<ApplicationUser>> GetUserByIdAsync(string userId);

        Task<Result<List<ApplicationUser>>> GetUsersByCompanyAndGroupAsync(int companyCode, int groupId);

        Task<Result<List<RoleModel>>> GetAllRolesAsync();

        Task<Result> CreateRoleAsync(RoleModel role);

        Task<Result> CreateUserWithRoleAsync(CreateUserWithRoleCommand command);

        Task<Result> UpdateUserRoleAsync(UpdateUserRoleCommand command);
    }
}
