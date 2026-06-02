namespace CloudAccounting.Wasm.Services.Repositories.Common
{
    public interface ILookupService
    {
        Task<Result<List<CompanyLookup>>> GetCompanyLookups();

        Task<Result<List<CostCenterLookupItem>>> GetCostCenterLookups(int companyCode);
    }
}
