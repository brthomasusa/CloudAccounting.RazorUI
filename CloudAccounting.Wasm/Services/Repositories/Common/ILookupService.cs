namespace CloudAccounting.Wasm.Services.Repositories.Common
{
    public interface ILookupService
    {
        Task<Result<List<CompanyLookup>>> GetCompanyLookups();

        Task<Result<List<CostCenterLookupItem>>> GetCostCenterLookups(int companyCode);

        Task<Result<List<FiscalYearLookupItem>>> RetrieveFiscalYearsAsync(int companyCode);

        Task<Result<List<FiscalPeriodLookupItem>>> RetrieveFiscalPeriodsAsync(int companyCode, int companyYear);
    }
}
