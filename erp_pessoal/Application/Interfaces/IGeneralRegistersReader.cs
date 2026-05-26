using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralRegistersReader
    {
        Task<IEnumerable<ReceiptDTO>> ReadReceiptsAsync(int id);
    }
}
