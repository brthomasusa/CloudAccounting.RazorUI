namespace CloudAccounting.Wasm.Models.Coa;

public record DeleteChartOfAccountCommand(int CompanyCode, string AccountCode);