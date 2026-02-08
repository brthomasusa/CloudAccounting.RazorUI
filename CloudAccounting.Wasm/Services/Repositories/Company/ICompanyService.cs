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

        Task<Result<CompanyWithFiscalPeriodsDto>> GetCompanyFiscalYearAsync(int companyCode);

        Task<Result<CompanyWithFiscalPeriodsDto>> CreateCompanyFiscalYearAsync(CreateFiscalYearCommand command);

        Task<Result<DateTime>> GetNextValidFiscalYearStartDateAsync(int companyCode);

        Task<Result> DeleteCompanyFiscalYearAsync(int companyCode, int fiscalYear);
    }
}
