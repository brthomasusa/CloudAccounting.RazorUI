namespace CloudAccounting.Wasm.Models.VoucherTypes
{
    public class VoucherTypeCommand
    {
        public int VoucherCode { get; set; }

        public string? VoucherType { get; set; }

        public string? VoucherTitle { get; set; }

        public byte VoucherClassification { get; set; }
    }
}
