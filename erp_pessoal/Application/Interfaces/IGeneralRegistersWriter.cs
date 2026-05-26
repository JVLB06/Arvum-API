using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralRegistersWriter
    {
        Task CreateReceiptAsync(ReceiptEntity receipt);
        Task UpdateReceiptAsync(ReceiptEntity receipt);
        Task DeleteReceiptAsync(int Id);
    }
}
