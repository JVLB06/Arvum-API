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
    }
}
