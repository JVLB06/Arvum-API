using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralDebtsReader
    {
        Task<IEnumerable<DebtDTO>> ReadDebtsAsync(int id);
    }
}
