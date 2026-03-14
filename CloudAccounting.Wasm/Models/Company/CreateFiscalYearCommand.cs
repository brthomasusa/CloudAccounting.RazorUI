namespace CloudAccounting.Wasm.Models.Company
{
    public class CreateFiscalYearCommand(
        int companyCode,
        int fiscalYear,
        DateTime startDate
    )
    {
        public int CompanyCode { get; set; } = companyCode;
        public int FiscalYear { get; set; } = fiscalYear;
        public DateTime StartDate { get; set; } = startDate;
    }


}
