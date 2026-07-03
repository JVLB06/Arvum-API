using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralReceiptsReader
    {
        Task<IEnumerable<ReceiptDTO>> ReadReceiptsAsync(int id);
    }
}
