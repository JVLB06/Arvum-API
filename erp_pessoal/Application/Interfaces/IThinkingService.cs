using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IThinkingService
    {
        Task<IEnumerable<PreferenceEntity>> GetPreferences(int userId);
        Task CreatePreference(PreferenceDTO preference, int userId);
        Task DeletePreference(int id, int userId);
        Task<SugestionsReponseEntity> GeneratePreferencesAsync(int userId);
    }
}
