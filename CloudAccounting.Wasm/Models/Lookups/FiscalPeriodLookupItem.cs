namespace CloudAccounting.Wasm.Models.Lookups;

public class FiscalPeriodLookupItem
{
    public byte CompanyMonthId { get; set; }

    public string CompanyMonthName { get; set; } = string.Empty;
}