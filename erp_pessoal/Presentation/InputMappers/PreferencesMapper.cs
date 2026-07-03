using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class PreferencesMapper
    {
        public static PreferenceDTO ToInput(PreferenceModel model)
        {
            return new PreferenceDTO
            {
                ExternalId = model.ExternalId,
                Exclude = model.Exclude,
                Reduce = model.Reduce,
                Block = model.Block
            };
        }
    }
}
