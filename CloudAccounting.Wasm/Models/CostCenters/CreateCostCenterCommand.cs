namespace CloudAccounting.Wasm.Models.CostCenters;

public class CreateCostCenterCommand
{
    public int CompanyCode { get; set; }

    public string? CostCenterCode { get; set; } = null!;

    public string? CostCenterTitle { get; set; }

    public byte CostCenterLevel { get; set; }
}