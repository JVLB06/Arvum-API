using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralInvestmentsReader
    {
        Task<IEnumerable<InvestmentDTO>> ReadActivesInvestmentsAsync(int id);
        Task<IEnumerable<InvestmentDTO>> ReadInactivesInvestmentsAsync(int id);
    }
}
