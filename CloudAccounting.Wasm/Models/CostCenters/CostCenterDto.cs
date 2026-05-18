namespace CloudAccounting.Wasm.Models.CostCenters;

public class CostCenterDto
{
    public int CompanyCode { get; set; }

    public string CostCenterCode { get; set; } = null!;

    public string? CostCenterTitle { get; set; }

    public byte CostCenterLevel { get; set; }
}