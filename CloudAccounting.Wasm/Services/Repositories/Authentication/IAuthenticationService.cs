using CloudAccounting.Wasm.Models;

namespace CloudAccounting.Wasm.Services.Repositories.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponseModel>> LoginAsync(LoginCommand request);
    }
}
