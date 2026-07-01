using Application.DTOs;

namespace Application.Interfaces
{
    public interface IThinkingReader
    {
        Task<IEnumerable<PreferenceDTO>> ReadPreferencesAsync(int userId);
        Task<PreferenceDTO> ReadPreferenceAsync(int userId, int mainId);
    }
}
