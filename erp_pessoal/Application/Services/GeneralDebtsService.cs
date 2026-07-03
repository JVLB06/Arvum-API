using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralDebtsService : IGeneralDebtsService
    {
        private readonly IGeneralDebtsReader _reader;

        private readonly IGeneralDebtsWriter _writer;

        public GeneralDebtsService(
            IGeneralDebtsReader reader,
            IGeneralDebtsWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<DebtEntity>> GetDebtsAsync(int id)
        {
            var connect = await _reader.ReadDebtsAsync(id);
            return connect.Select(debt => new DebtEntity(
                debt.Id,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate,
                debt.Paid,
                null
            ));
        }

        public async Task RegisterDebtAsync(DebtDTO debt, int userId)
        {
            await _writer.CreateDebtAsync(new DebtEntity(
                debt.Id,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate,
                debt.Paid,
                userId
            ));
        }

        public async Task UpdateDebtAsync(DebtDTO debt, int userId)
        {
            await _writer.UpdateDebtAsync(new DebtEntity(
                debt.Id,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate,
                debt.Paid,
                userId
            ));
        }
        public async Task DeleteDebtAsync(int id)
        {
            await _writer.InactivateDebtAsync(id);
        }

        public async Task PayDebtAsync(int id)
        {
            await _writer.PayDebtAsync(id);
        }

        public async Task<IEnumerable<DebtEntity>> GetPaidDebtsAsync(int userId)
        {
            var connect = await _reader.ReadInactiveDebtsAsync(userId);
            return connect.Select(debt => new DebtEntity(
                debt.Id,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate,
                debt.Paid,
                userId
            ));
        }
    }
}
