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

        Task<Result<CompanyDetail>> CreateCompanyAsync(CompanyDetail company);

        Task<Result> UpdateCompanyAsync(CompanyDetail company);

        Task<Result> DeleteCompanyAsync(int companyCode);
    }
}
