using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralInvestmentsService : IGeneralInvestmentsService
    {
        private readonly IGeneralInvestmentsReader _reader;

        private readonly IGeneralInvestmentsWriter _writer;

        public GeneralInvestmentsService(
            IGeneralInvestmentsReader reader,
            IGeneralInvestmentsWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<InvestmentEntity>> GetActiveInvestmentsAsync(int id)
        {
            var connect = await _reader.ReadActivesInvestmentsAsync(id);

            return connect.Select(investment => new InvestmentEntity(
                investment.Id,
                investment.Description,
                investment.Value,
                investment.Interest,
                investment.InitialDate,
                investment.ReceiveDate,
                investment.ReceivedValue,
                null
            ));
        }

        public async Task<IEnumerable<InvestmentEntity>> GetInactiveInvestmentsAsync(int id)
        {
            var connect = await _reader.ReadInactivesInvestmentsAsync(id);

            return connect.Select(investment => new InvestmentEntity(
                investment.Id,
                investment.Description,
                investment.Value,
                investment.Interest,
                investment.InitialDate,
                investment.ReceiveDate,
                investment.ReceivedValue,
                null
            ));
        }

        public async Task CreateInvestmentAsync(InvestmentDTO investment, int userId)
        {
            await _writer.CreateInvestmentAsync(new InvestmentEntity(
                investment.Id,
                investment.Description,
                investment.Value,
                investment.Interest,
                investment.InitialDate,
                investment.ReceiveDate,
                investment.ReceivedValue,
                userId
                ));
        }

        public async Task UpdateInvestmentAsync(InvestmentDTO investment, int userId)
        {
            await _writer.UpdateInvestmentAsync(new InvestmentEntity(
                investment.Id,
                investment.Description,
                investment.Value,
                investment.Interest,
                investment.InitialDate,
                investment.ReceiveDate,
                investment.ReceivedValue,
                userId));
        }

        public async Task DeleteInvestmentAsync(int id, int userId)
        {
            await _writer.DeleteInvestmentAsync(new DeleteInvestmentEntity(id, userId));
        }

        public async Task FinishInvestmentAsync(FinishInvestmentDTO investment, int userId)
        {
            await _writer.FinishInvestmentAsync(new FinishInvestmentEntity(
                investment.Id,
                userId,
                investment.ReceiveDate,
                investment.ReceivedValue));
        }
    }
}
