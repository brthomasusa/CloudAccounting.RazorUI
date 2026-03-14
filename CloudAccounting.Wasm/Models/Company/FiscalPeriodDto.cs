namespace CloudAccounting.Wasm.Models.Company
{
    public class FiscalPeriodDto
    {
        public int MonthId { get; set; }
        public string? MonthName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool PeriodClosed { get; set; }
    }
}

