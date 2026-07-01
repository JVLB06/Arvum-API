using Domain.Entities;

namespace Application.Interfaces
{
    public interface IThinkingWriter
    {
        Task SetPreferenceAsync(PreferenceEntity preference);
        Task PutPreferenceAsync(PreferenceEntity preference);
        Task DeletePreferenceAsync(int userId, int id);
    }
}
