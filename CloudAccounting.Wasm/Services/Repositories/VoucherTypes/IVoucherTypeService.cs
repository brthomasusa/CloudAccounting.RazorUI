using CloudAccounting.Wasm.Models.VoucherTypes;

namespace CloudAccounting.Wasm.Services.Repositories.VoucherTypes
{
    public interface IVoucherTypeService
    {
        Task<Result<List<VoucherTypeDto>>> RetrieveAllAsync();

        Task<Result<VoucherTypeDto>> RetrieveAsync(int voucherCode);

        Task<Result<VoucherTypeCommand>> CreateAsync(VoucherTypeCommand v);

        Task<Result<VoucherTypeCommand>> UpdateAsync(VoucherTypeCommand v);

        Task<Result> DeleteAsync(int voucherCode);
    }
}
