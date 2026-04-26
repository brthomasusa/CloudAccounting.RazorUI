namespace CloudAccounting.Wasm.Models.Common
{
    public class MetaData(int pageNumber, int pageSize, int totalRecords)
    {
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalRecords { get; set; } = totalRecords;
    }
}
