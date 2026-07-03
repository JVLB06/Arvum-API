using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class ConnectionMapper
    {
        public static ConnectionDTO ToDTO(ConnectionModel model)
        {
            return new ConnectionDTO
            {
                Id = model.Id,
                Email = model.Name,
                Authenticated = null
            };
        }
    }
}
