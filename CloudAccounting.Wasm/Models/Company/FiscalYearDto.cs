namespace CloudAccounting.Wasm.Models.Company
{
    public record FiscalYearDto
    (
        int CompanyCode,
        string CompanyName,
        int Year,
        DateTime FiscalYearStartDate,
        DateTime FiscalYearEndDate,
        bool IsInitialFiscalYear,
        bool IsFiscalYearClosed,
        bool HasTransactions,
        DateTime? TemporaryYearEndProcessLastExecuted,
        List<FiscalPeriodDto> FiscalPeriods
    );
}
