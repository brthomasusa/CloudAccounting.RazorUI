namespace CloudAccounting.Wasm.Models.Coa;

public class CreateChartOfAccountCommand
{
    private string _accountClassification = null!;

    public int CompanyCode { get; set; }

    public string? LevelOne
    {
        get;
        set
        {
            _accountClassification = value switch
            {
                "1" => "Asset",
                "2" => "Liability",
                "3" => "Equity",
                "4" => "Revenue",
                "5" => "Expense",
                _ => null!
            };
            field = value;
        }
    }

    public string? LevelTwo { get; set; }
    public string? LevelThree { get; set; }
    public string? LevelFour { get; set; }

    public string? AccountTitle { get; set; }

    public string AccountClassification => _accountClassification;

    public string? AccountType { get; set; }

    public string? CostCenterCode { get; set; }
}