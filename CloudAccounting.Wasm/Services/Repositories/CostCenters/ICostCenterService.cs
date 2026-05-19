using CloudAccounting.Wasm.Models.CostCenters;

namespace CloudAccounting.Wasm.Services.Repositories.CostCenters;

public interface ICostCenterService
{
    Task<Result<List<CostCenterDto>>> RetrieveAllAsync(int companyCode);

    Task<Result<CostCenterDto>> RetrieveByIdAsync(int companyCode, string costCenterCode);

    Task<Result<CostCenterDto>> CreateAsync(CreateCostCenterCommand command);

    Task<Result> UpdateAsync(UpdateCostCenterCommand command);

    Task<Result> DeleteAsync(int companyCode, string costCenterCode);
}