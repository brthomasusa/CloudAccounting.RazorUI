namespace CloudAccounting.Wasm.Models.VoucherTypes
{
    public class VoucherTypeDto
    {
        public int VoucherCode { get; set; }

        public string? VoucherType { get; set; }

        public string? VoucherTitle { get; set; }

        public byte VoucherClassification { get; set; }

        public string? VoucherClassificationName        // For display in the datagrid, not stored in the database
        { 
            get
            {
                return VoucherClassification switch
                {
                    1 => "Payment Voucher",
                    2 => "Receipt Voucher",
                    3 => "Journal Voucher",
                    _ => null
                };
            }
        }
    }
}
