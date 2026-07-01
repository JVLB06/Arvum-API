using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ThinkingService : IThinkingService
    {
        private readonly IThinkingReader _reader;
        private readonly IThinkingWriter _writer;

        public ThinkingService(IThinkingReader reader, IThinkingWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<PreferenceEntity>> GetPreferences(int userId)
        {
            var connect = await _reader.ReadPreferencesAsync(userId);

            return connect.Select(preference => new PreferenceEntity(
                preference.UserId,
                preference.Id,
                preference.ExternalId,
                preference.Exclude,
                preference.Reduce,
                preference.Block,
                preference.Name));
        }

        public async Task CreatePreference(PreferenceDTO preference, int userId)
        {
            var connect = await _reader.ReadPreferenceAsync(userId, (int)(preference.Id is null ? 0 : preference.Id));

            PreferenceEntity input = new PreferenceEntity(
                    preference.UserId,
                    preference.Id,
                    preference.ExternalId,
                    preference.Exclude,
                    preference.Reduce,
                    preference.Block,
                    null);

            if (connect is null)
            {
                await _writer.SetPreferenceAsync(input);
            }
            else
            {
                await _writer.PutPreferenceAsync(input);
            }
        }

        public async Task DeletePreference(int id, int userId)
        {
            await _writer.DeletePreferenceAsync(id, userId);
        }

        public async Task<SugestionsReponseEntity> GeneratePreferencesAsync(int userId)
        {
            IEnumerable<GeneralInfoDTO> debts = await _reader.ReadDebtTotalAsync(userId);
            IEnumerable<GeneralInfoDTO> receipts = await _reader.ReadReceiptTotalAsync(userId);
            IEnumerable<GeneralInfoDTO> expenses = await _reader.ReadExpensesTotalAsync(userId);
            IEnumerable<PreferencesInfoDTO> exclusions = await _reader.ReadExclusionsAsync(userId);
            IEnumerable<PreferencesInfoDTO> reductions = await _reader.ReadReductionsAsync(userId);

            IEnumerable<SugestionsEntity> reduction = reductions.Select(dto =>
                new SugestionsEntity(
                    id: dto.Id,
                    name: dto.Name,
                    minValue: dto.MinValue,
                    maxValue: dto.MaxValue
                )
            );

            IEnumerable<SugestionsEntity> exclusion = exclusions.Select(dto =>
                new SugestionsEntity(
                    id: dto.Id,
                    name: dto.Name,
                    minValue: dto.MinValue,
                    maxValue: dto.MaxValue
                )
            );

            return (new SugestionsReponseEntity(
                userId,
                debts.FirstOrDefault()?.Total ?? 0, //Debts
                receipts.FirstOrDefault()?.Total ?? 0, //Receipts
                expenses.FirstOrDefault(x => x.Kind == "Fix")?.Total ?? 0, //Fixed Expenses
                expenses.FirstOrDefault(x => x.Kind == "Var")?.Total ?? 0,
                exclusion,
                reduction));
        }
    }
}
