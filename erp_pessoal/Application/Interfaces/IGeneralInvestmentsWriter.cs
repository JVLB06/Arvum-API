using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralInvestmentsWriter
    {
        Task CreateInvestmentAsync(InvestmentEntity investment);
        Task UpdateInvestmentAsync(InvestmentEntity investment);
        Task DeleteInvestmentAsync(DeleteInvestmentEntity investment);
        Task FinishInvestmentAsync(FinishInvestmentEntity investment);
    }
}
