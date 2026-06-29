using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class SpecificRegistersService : ISpecificRegistersService
    {
        public readonly ISpecificRegistersReader _reader;
        public readonly ISpecificRegistersWriter _writer;
        public readonly ISpecificRegistersService _service;

        public SpecificRegistersService(ISpecificRegistersReader reader, ISpecificRegistersWriter writer, ISpecificRegistersService service)
        {
            _reader = reader;
            _writer = writer;
            _service = service;
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
                extract.Balance,
                null
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

        public async Task<int> SetExtractAsync(ExtractDTO main, int userId)
        {
            ExtractEntity payload = new ExtractEntity(
                main.Id,
                userId,
                main.Name,
                main.Value,
                main.ExtractDate,
                main.Kind,
                0,
                main.ExternalId);

            int newId = await _writer.CreateMainExtractAsync(payload);
            int newSpecificId;

            switch (main.Kind)
            {
                case "gasto":
                    newSpecificId =  await _writer.CreateExpenseExtractAsync(payload, newId);
                    break;
                case "divida":
                    newSpecificId = await _writer.CreateDebtExtractAsync(payload, newId);
                    break;
                case "meta":
                    newSpecificId = await _writer.CreateGoalExtractAsync(payload, newId);
                    break;
                case "investimento":
                    newSpecificId = await _writer.CreateInvestmentExtractAsync(payload, newId);
                    break;
                case "renda":
                    newSpecificId = await _writer.CreateReceiptExtractAsync(payload, newId);
                    break;
                default:
                    return 500;       
            }

            await _service.CalculateBalancesAsync(userId, newSpecificId);
            return newSpecificId;
        }

        public async Task UpdateExtractAsync(ExtractDTO main, int userId)
        {
            ExtractEntity payload = new ExtractEntity(
                main.Id,
                userId,
                main.Name,
                main.Value,
                main.ExtractDate,
                main.Kind,
                0,
                main.ExternalId);

            await _writer.UpdateMainExtractAsync(payload);

            switch (main.Kind)
            {
                case "gasto":
                    await _writer.UpdateExpenseExtractAsync(payload);
                    break;
                case "divida":
                    await _writer.UpdateDebtExtractAsync(payload);
                    break;
                case "meta":
                    await _writer.UpdateGoalExtractAsync(payload);
                    break;
                case "investimento":
                    await _writer.UpdateInvestmentExtractAsync(payload);
                    break;
                case "renda":
                    await _writer.UpdateReceiptExtractAsync(payload);
                    break;
            }

            await _service.CalculateBalancesAsync(userId, main.Id);
        }

        public async Task DeleteExtractAsync(ExtractDeleteDTO main, int userId)
        {
            switch (main.Kind)
            {
                case "gasto":
                    await _writer.DeleteExpenseExtractAsync(main.Id, userId);
                    break;
                case "divida":
                    await _writer.DeleteDebtExtractAsync(main.Id, userId);
                    break;
                case "meta":
                    await _writer.DeleteGoalExtractAsync(main.Id, userId);
                    break;
                case "investimento":
                    await _writer.DeleteInvestmentExtractAsync(main.Id, userId);
                    break;
                case "renda":
                    await _writer.DeleteReceiptExtractAsync(main.Id, userId);
                    break;
            }

            await _writer.DeleteMainExtractAsync(main.Id, userId);
            await _service.CalculateBalancesAsync(userId, main.Id);
        }

        public async Task CalculateBalancesAsync(int userId, int entryId)
        {
            DateTime extractDate = await _reader.GetExtractDateByIdAsync(userId, entryId);

            if (extractDate == DateTime.MinValue) return;

            decimal lastGroupedBalance = await _reader.GetLastBalanceAsync(userId, extractDate);

            var entrys = await _reader.GetNextEntrysAsync(userId, extractDate);

            if (!entrys.Any()) return;

            var balancesToUpdate = new List<ExtractBalanceEntity>();

            foreach (var entry in entrys)
            {
                lastGroupedBalance += entry.Balance;

                balancesToUpdate.Add(new ExtractBalanceEntity(entry.Id, lastGroupedBalance));
            }

            await _writer.UpdateMultipleBalanceAsync(balancesToUpdate);
        }
    }
}
