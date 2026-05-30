namespace CloudAccounting.Wasm.Models.Coa;

public record UpdateChartOfAccountCommand(
    int CompanyCode,
    string AccountCode,
    string AccountTitle,
    string AccountType,
    string CostCenterCode);