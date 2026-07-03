using Application.DTOs;

namespace Application.Interfaces
{
    public interface IThinkingReader
    {
        Task<IEnumerable<PreferenceDTO>> ReadPreferencesAsync(int userId);
        Task<PreferenceDTO> ReadPreferenceAsync(int userId, int mainId);
        Task<IEnumerable<GeneralInfoDTO>> ReadDebtTotalAsync(int userId);
        Task<IEnumerable<GeneralInfoDTO>> ReadReceiptTotalAsync(int userId);
        Task<IEnumerable<GeneralInfoDTO>> ReadExpensesTotalAsync(int userId);
        Task<IEnumerable<PreferencesInfoDTO>> ReadExclusionsAsync(int userId);
        Task<IEnumerable<PreferencesInfoDTO>> ReadReductionsAsync(int userId);
    }
}
