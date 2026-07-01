using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class PreferencesService : IPreferencesService
    {
        private readonly IThinkingReader _reader;
        private readonly IThinkingWriter _writer;

        public PreferencesService(IThinkingReader reader, IThinkingWriter writer)
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
            var connect = await _reader.ReadPreferenceAsync(userId, preference.Id);

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
    }
}
