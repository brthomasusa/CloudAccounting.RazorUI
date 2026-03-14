using CloudAccounting.Wasm.Models.Company;

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

        Task<Result<FiscalYearDto>> GetCompanyFiscalYearAsync(int companyCode);

        Task<Result<FiscalYearDto>> GetCompanyFiscalYearAsync(int companyCode, int fiscalYear);

        Task<Result<FiscalYearDto>> CreateCompanyFiscalYearAsync(CreateFiscalYearCommand command);

        Task<Result> DeleteCompanyFiscalYearAsync(int companyCode, int fiscalYear);        
    }
}
