using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralRegistersService : IGeneralRegistersService
    {
        private readonly IGeneralRegistersReader _reader;

        //private readonly IAuthWriter _writer;

        public GeneralRegistersService(
            IGeneralRegistersReader reader)
            //IAuthWriter writer)
        {
            _reader = reader;
            //_writer = writer;
        }

        public async Task <ReceiptEntity> GetReceiptsAsync(int id)
        {
            var connect = await _reader.ReadReceiptsAsync(id);

            return new ReceiptEntity(
                    connect.Id,
                    connect.Description,
                    connect.MinValue,
                    connect.MaxValue,
                    connect.PaymentDate);
        }
    }
}
