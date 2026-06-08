namespace CloudAccounting.Wasm.Models.Authentication;

public class UpdateUserFiscalPeriodCommand(int companyCode, Int16 companyYear, byte companyMonthId)
{
    public int CompanyCode { get; } = companyCode;
    public Int16 CompanyYear { get; set; } = companyYear;
    public byte CompanyMonthId { get; set; } = companyMonthId;
}