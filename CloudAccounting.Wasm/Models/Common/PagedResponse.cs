namespace CloudAccounting.Wasm.Models.Common;

public record PagedResponse<T>(List<T> Data, int PageNumber, int PageSize, int TotalRecords)
{
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
}