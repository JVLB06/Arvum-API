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
                debt.Paid
            ));
        }
    }
}
