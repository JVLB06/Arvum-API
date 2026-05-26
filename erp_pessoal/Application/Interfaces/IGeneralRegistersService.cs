using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralRegistersService
    {
        Task <ReceiptEntity> GetReceiptsAsync(int id);
    }
}
