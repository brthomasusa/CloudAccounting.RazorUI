using CloudAccounting.Wasm.Models.Authentication;

namespace CloudAccounting.Wasm.Services.Repositories.FiscalPeriod;

public interface IFiscalPeriodService
{
    Task<Result> UpdateCurrentFiscalPeriodAsync(UpdateUserFiscalPeriodCommand command);
}