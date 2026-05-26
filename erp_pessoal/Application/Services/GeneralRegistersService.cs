using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralRegistersService : IGeneralRegistersService
    {
        private readonly IGeneralRegistersReader _reader;

        private readonly IGeneralRegistersWriter _writer;

        public GeneralRegistersService(
            IGeneralRegistersReader reader,
            IGeneralRegistersWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<ReceiptEntity>> GetReceiptsAsync(int id)
        {
            var connect = await _reader.ReadReceiptsAsync(id);

            return connect.Select(receipt => new ReceiptEntity(
                receipt.Id,
                receipt.Description,
                receipt.MinValue,
                receipt.MaxValue,
                receipt.PaymentDate
            ));
        }

        public async Task CreateReceiptAsync(ReceiptDTO receipt, int userId)
        {
            await _writer.CreateReceiptAsync(new ReceiptEntity(
                receipt.Id,
                receipt.Description,
                receipt.MinValue,
                receipt.MaxValue,
                receipt.PaymentDate,
                userId));
        }

        public async Task UpdateReceiptAsync(ReceiptDTO receipt, int userId)
        {
            await _writer.UpdateReceiptAsync(new ReceiptEntity(
                receipt.Id,
                receipt.Description,
                receipt.MinValue,
                receipt.MaxValue,
                receipt.PaymentDate,
                userId));
        }

        public async Task DeleteReceiptAsync(int receiptId)
        {
            await _writer.DeleteReceiptAsync(receiptId);
        }
    }
}
