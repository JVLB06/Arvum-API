using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralInvestmentsService
    {
        Task<IEnumerable<InvestmentEntity>> GetActiveInvestmentsAsync(int id);
        Task<IEnumerable<InvestmentEntity>> GetInactiveInvestmentsAsync(int id);
        Task CreateInvestmentAsync(InvestmentDTO investment, int userId);
        Task UpdateInvestmentAsync(InvestmentDTO investment, int userId);
        Task DeleteInvestmentAsync(int id, int userId);
        Task FinishInvestmentAsync(FinishInvestmentDTO investment, int userId);
    }
}
