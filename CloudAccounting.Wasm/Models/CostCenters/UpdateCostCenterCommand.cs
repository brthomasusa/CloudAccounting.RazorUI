namespace CloudAccounting.Wasm.Models.CostCenters;

public record UpdateCostCenterCommand(
    int CompanyCode,
    string CostCenterCode,
    string CostCenterTitle
);