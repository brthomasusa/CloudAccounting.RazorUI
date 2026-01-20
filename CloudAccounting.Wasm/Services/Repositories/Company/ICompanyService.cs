using CloudAccounting.Wasm.Models;
using CloudAccounting.Wasm.Models.Company;
using CloudAccounting.Wasm.Utilities;

namespace CloudAccounting.Wasm.Services.Repositories.Company
{
    public interface ICompanyService
    {
        Task<Result<CompanyDetail>> GetCompanyByIdAsync(int companyCode);

        Task<Result<List<CompanyDetail>>> GetCompaniesAsync
        (
            int pageNumber,
            int pageSize
        );
    }
}
