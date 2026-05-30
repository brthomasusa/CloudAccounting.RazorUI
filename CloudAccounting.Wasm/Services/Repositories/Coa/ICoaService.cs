
namespace CloudAccounting.Wasm.Services.Repositories.Coa;

public interface ICoaService
{
    Task<Result<PagedResponse<ChartOfAccountDto>>> RetrieveAllAsync(int pageNumber, int pageSize, int companyCode);

    Task<Result<PagedResponse<ChartOfAccountDto>>> RetrieveAllAsync(int pageNumber, int pageSize, int companyCode, string accountNumber);

    Task<Result<ChartOfAccountDto>> RetrieveByIdAsync(int companyCode, string accountNumber);

    Task<Result<ChartOfAccountDto>> CreateAsync(CreateChartOfAccountCommand command);

    Task<Result> UpdateAsync(UpdateChartOfAccountCommand command);

    Task<Result> DeleteAsync(DeleteChartOfAccountCommand command);
}