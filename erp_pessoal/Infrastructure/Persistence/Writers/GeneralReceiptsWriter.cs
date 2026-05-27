using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class GeneralReceiptsWriter : IGeneralReceiptsWriter
    {
        #region Rendas
        public async Task CreateReceiptAsync(ReceiptEntity receipt)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                INSERT INTO 
                    rendas (user_id, nome, vlr_min, vlr_max, data_pag, ativo) 
                VALUES 
                    (@UserId, @Description, @MinValue, @MaxValue, @PaymentDate, TRUE)";

            await conn.ExecuteAsync(sql, new
            {
                receipt.UserId,
                receipt.Description,
                receipt.MinValue,
                receipt.MaxValue,
                receipt.PaymentDate
            });
        }

        public async Task UpdateReceiptAsync(ReceiptEntity receipt)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                UPDATE rendas
                SET 
                    nome = @Description,
                    vlr_min = @MinValue,
                    vlr_max = @MaxValue,
                    data_pag = @PaymentDate
                WHERE id_renda = @Id";
            await conn.ExecuteAsync(sql, new
            {
                receipt.Description,
                receipt.MinValue,
                receipt.MaxValue,
                receipt.PaymentDate,
                receipt.Id
            });
        }

        public async Task DeleteReceiptAsync(int Id)
        {
            using var conn = MainRepository.CreateConnection();
            
            const string sql = @"
                UPDATE rendas
                SET ativo = FALSE
                WHERE id_renda = @Id";

            await conn.ExecuteAsync(sql, new { Id });
        }
        #endregion
    }
}