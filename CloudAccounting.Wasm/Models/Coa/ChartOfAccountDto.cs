namespace CloudAccounting.Wasm.Models.Coa;

public class ChartOfAccountDto
{
    public ChartOfAccountDto() { }

    public int CompanyCode { get; set; }

    public string AccountCode { get; set; } = null!;

    public string? AccountTitle { get; set; }

    public int AccountLevel { get; set; }

    public string? AccountClassification { get; set; }

    public string? AccountType { get; set; }

    public string? CostCenterCode { get; set; }


    // public ICollection<BankOpeningStatement> BankOpeningStatements { get; set; } = [];

    // public ICollection<TransactionDetail> TransactionDetails { get; set; } = [];     
}