using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPreferencesService
    {
        Task<IEnumerable<PreferenceEntity>> GetPreferences(int userId);
        Task CreatePreference(PreferenceDTO preference, int userId);
        Task DeletePreference(int id, int userId);
    }
}
