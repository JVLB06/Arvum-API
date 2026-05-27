using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralReceiptsWriter
    {
        Task CreateReceiptAsync(ReceiptEntity receipt);
        Task UpdateReceiptAsync(ReceiptEntity receipt);
        Task DeleteReceiptAsync(int Id);
    }
}
