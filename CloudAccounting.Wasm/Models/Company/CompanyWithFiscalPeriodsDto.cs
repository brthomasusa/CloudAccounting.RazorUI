namespace CloudAccounting.Wasm.Models.Company
{
    public class CompanyWithFiscalPeriodsDto
    {
        public int CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public int FiscalYear { get; set; }
        public bool IsInitialYear { get; set; }
        public int Transactions { get; set; }
        public List<FiscalPeriodDto> FiscalPeriods { get; set; } = [];
    }
}
