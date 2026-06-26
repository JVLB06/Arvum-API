using Application.Interfaces;
using Domain.Entities;
using System.Reflection.PortableExecutable;

namespace Application.Services
{
    public class SpecificRegistersService : ISpecificRegistersService
    {
        public readonly ISpecificRegistersReader _reader;
        public readonly ISpecificRegistersWriter _writer;
        public SpecificRegistersService(ISpecificRegistersReader reader, ISpecificRegistersWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<ExtractEntity>> GetExtractAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadExtractByUser(userId, initialDate, endDate);
            return connect.Select(extract => new ExtractEntity(
                extract.Id,
                userId,
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.Kind,
                extract.Balance
            ));
        }

        public async Task<IEnumerable<SpecificGoalEntity>> GetGoalPaymentsAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadGoalEntryByUser(userId, initialDate, endDate);
            return connect.Select(extract => new SpecificGoalEntity(
                extract.Id,
                userId,
                extract.SpecificId,
                extract.ExtractDate,
                extract.Description,
                extract.EntryValue,
                extract.GoalId,
                extract.GoalName,
                extract.FullGoalValue,
                extract.GoalDate,
                extract.Progress,
                extract.Balance
            ));
        }

        public async Task<IEnumerable<SpecificExpensesEntity>> GetExpensePayementsAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadExpenseEntryByUser(userId, initialDate, endDate);
            return connect.Select(extract => new SpecificExpensesEntity(
                extract.Id,
                userId,
                extract.SpecificId,
                extract.ExtractDate,
                extract.Description,
                extract.EntryValue,
                extract.ExpenseId,
                extract.ExpenseName,
                extract.ExpenseValue,
                extract.ExpenseDate,
                extract.Variable,
                extract.Balance));
        }

        public async Task<IEnumerable<SpecificDebtEntity>> GetDebtPaymentsAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadDebtsEntryByUser(userId, initialDate, endDate);
            return connect.Select(extract => new SpecificDebtEntity(
                extract.Id,
                userId,
                extract.SpecificId,
                extract.ExtractDate,
                extract.Description,
                extract.EntryValue,
                extract.DebtId,
                extract.DebtName,
                extract.DebtValue,
                extract.DebtDate,
                extract.DebtEndDate,
                extract.Balance));
        }

        public async Task<IEnumerable<SpecificInvestmentEntity>> GetInvestmentPaymentsAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadInvestmentsEntryByUser(userId, initialDate, endDate);
            return connect.Select(extract => new SpecificInvestmentEntity(
                extract.Id,
                userId,
                extract.SpecificId,
                extract.ExtractDate,
                extract.Description,
                extract.Value,
                extract.InvestId,
                extract.InvestName,
                extract.InvestValue,
                extract.Interest,
                extract.InvestDate,
                extract.Balance));
        }

        public async Task<IEnumerable<SpecificReceiptEntity>> GetReceiptPaymentsAsync(int userId, DateTime initialDate, DateTime endDate)
        {
            var connect = await _reader.ReadReceiptsEntryByUser(userId, initialDate, endDate);
            return connect.Select(extract => new SpecificReceiptEntity(
                extract.Id,
                userId,
                extract.SpecificId,
                extract.ExtractDate,
                extract.Description,
                extract.Value,
                extract.ReceiptId,
                extract.ReceiptValue,
                extract.ReceiptDate,
                extract.Balance));
        }
    }
}
