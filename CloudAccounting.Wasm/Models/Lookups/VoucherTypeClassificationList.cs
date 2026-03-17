namespace CloudAccounting.Wasm.Models.Lookups
{
    public static class VoucherTypeClassificationList
    {
        public static readonly List<VoucherTypeClassification> Classifications =
        [
            new VoucherTypeClassification { Classification = 1, ClassificationName = "Payment Voucher" },
            new VoucherTypeClassification { Classification = 2, ClassificationName = "Receipt Voucher" },
            new VoucherTypeClassification { Classification = 3, ClassificationName = "Journal Voucher" }
        ];
    }
}
