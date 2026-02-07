namespace CloudAccounting.Wasm.Models.Company
{
    public class CreateFiscalYearCommand(
        int companyCode,
        int fiscalYear,
        int startMonthNumber
        )
    {
        public int CompanyCode { get; set; } = companyCode;
        public int FiscalYear { get; set; } = fiscalYear;
        public int StartMonthNumber { get; set; } = startMonthNumber;
    }


}
