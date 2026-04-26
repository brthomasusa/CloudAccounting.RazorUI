namespace CloudAccounting.Wasm.Models.Common
{
    public class DocumentPage<T>
    {
        public MetaData? MetaData { get; set; }
        public List<T> Data { get; set; } = [];
    }
}
