using Domain.Entities;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralRegistersService
    {
        Task <ReceiptEntity> GetReceiptsAsync(int id);
        Task CreateReceiptAsync(ReceiptDTO receipt, int userId);
        Task UpdateReceiptAsync(ReceiptDTO receipt, int userId);
        Task DeleteReceiptAsync(int receiptId);
    }
}
