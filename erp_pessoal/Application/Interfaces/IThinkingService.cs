using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IThinkingService
    {
        Task<IEnumerable<PreferenceEntity>> GetPreferences(int userId);
        Task CreatePreference(PreferenceDTO preference, int userId);
        Task DeletePreference(int id, int userId);
        /// <summary>
        /// Retorna objetos anonimamente serializados como JSON com as chaves:
        /// pensamentos (string[]), reducoes [{gastoId, nome, valorAtual, valorSugerido}],
        /// exclusoes [{gastoId, nome, valorAtual, valorSugerido}], comparativo {renda, gastos, ...}.
        /// </summary>
        Task<object> GeneratePreferencesAsync(int userId);
    }
}
