using Npgsql;
using erp_pessoal.Models;

namespace erp_pessoal.Controllers
{
    public class ThinkingUtils
    {
        public ThinkingResponseModel GerarSugestoes(int userId)
        {
            ThinkingResponseModel response = new();

            response.Indicadores = new IndicadoresModel();

            response.Indicadores.Endividamento =
                IndiceEndividamento(userId);

            response.Indicadores.Gastos =
                RelacaoGastos(userId);

            response.Indicadores.IndiceSaude =
                SaudeRenda(userId);

            response.Pensamentos =
                GerarPensamentos(response.Indicadores);

            response.Reducoes =
                GerarReducoes(userId);

            response.Exclusoes =
                GerarExclusoes(userId);

            return response;
        }

        public float IndiceEndividamento(int id)
        {
            using var conn =
                new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var rendaCmd = new NpgsqlCommand(@"
                SELECT COALESCE(SUM(valor),0)
                FROM rendas
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            var dividaCmd = new NpgsqlCommand(@"
                SELECT COALESCE(SUM(valor),0)
                FROM divida
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            rendaCmd.Parameters.AddWithValue("@id", id);
            dividaCmd.Parameters.AddWithValue("@id", id);

            float renda =
                Convert.ToSingle(rendaCmd.ExecuteScalar());

            float divida =
                Convert.ToSingle(dividaCmd.ExecuteScalar());

            if (renda == 0)
                return 0;

            return (divida / renda) * 100;
        }

        public RelacaoGastosModel RelacaoGastos(int id)
        {
            using var conn =
                new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            RelacaoGastosModel model = new();

            var rendaCmd = new NpgsqlCommand(@"
                SELECT COALESCE(SUM(valor),0)
                FROM rendas
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            rendaCmd.Parameters.AddWithValue("@id", id);

            float renda =
                Convert.ToSingle(rendaCmd.ExecuteScalar());

            var fixCmd = new NpgsqlCommand(@"
                SELECT
                    COALESCE(SUM(vlr_min),0),
                    COALESCE(SUM(vlr_max),0)
                FROM gastos
                WHERE user_id = @id
                AND ativo = TRUE
                AND fixvar = TRUE
            ", conn);

            fixCmd.Parameters.AddWithValue("@id", id);

            float fixMin = 0;
            float fixMax = 0;

            using (var rd = fixCmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    fixMin = rd.GetFloat(0);
                    fixMax = rd.GetFloat(1);
                }
            }

            var varCmd = new NpgsqlCommand(@"
                SELECT
                    COALESCE(SUM(vlr_min),0),
                    COALESCE(SUM(vlr_max),0)
                FROM gastos
                WHERE user_id = @id
                AND ativo = TRUE
                AND fixvar = FALSE
            ", conn);

            varCmd.Parameters.AddWithValue("@id", id);

            float varMin = 0;
            float varMax = 0;

            using (var rd = varCmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    varMin = rd.GetFloat(0);
                    varMax = rd.GetFloat(1);
                }
            }

            if (renda == 0)
                return model;

            float mediaFix = (fixMin + fixMax) / 2;
            float mediaVar = (varMin + varMax) / 2;

            model.PorcentagemGastosFixos =
                (mediaFix / renda) * 100;

            model.PorcentagemGastosVariaveis =
                (mediaVar / renda) * 100;

            return model;
        }

        public SaudeRendaModel SaudeRenda(int id)
        {
            using var conn =
                new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var rendaCmd = new NpgsqlCommand(@"
                SELECT COALESCE(SUM(valor),0)
                FROM rendas
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            rendaCmd.Parameters.AddWithValue("@id", id);

            float renda =
                Convert.ToSingle(rendaCmd.ExecuteScalar());

            var gastoCmd = new NpgsqlCommand(@"
                SELECT
                    COALESCE(SUM(vlr_min),0),
                    COALESCE(SUM(vlr_max),0)
                FROM gastos
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            gastoCmd.Parameters.AddWithValue("@id", id);

            float min = 0;
            float max = 0;

            using (var rd = gastoCmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    min = rd.GetFloat(0);
                    max = rd.GetFloat(1);
                }
            }

            float media = (min + max) / 2;

            float indice = renda == 0
                ? 0
                : (media / renda) * 100;

            int recomendacoes = 0;

            if (indice >= 100)
                recomendacoes = 3;
            else if (indice >= 70)
                recomendacoes = 2;
            else if (indice >= 55)
                recomendacoes = 1;

            return new SaudeRendaModel
            {
                IndiceSaudeFinanceira = indice,
                Recomendacoes = recomendacoes
            };
        }

        public List<string> GerarPensamentos(
            IndicadoresModel indicadores)
        {
            List<string> pensamentos = new();

            if (indicadores.Endividamento >= 50)
            {
                pensamentos.Add(
                    "Seu índice de endividamento está alto."
                );
            }

            if (indicadores.Gastos.PorcentagemGastosFixos >= 70)
            {
                pensamentos.Add(
                    "Seus gastos fixos estão muito elevados."
                );
            }

            if (indicadores.Gastos.PorcentagemGastosVariaveis >= 30)
            {
                pensamentos.Add(
                    "Seus gastos variáveis estão acima do ideal."
                );
            }

            switch (indicadores.IndiceSaude.Recomendacoes)
            {
                case 3:
                    pensamentos.Add(
                        "Sua saúde financeira está comprometida."
                    );
                    break;

                case 2:
                    pensamentos.Add(
                        "Você está próximo do limite saudável de gastos."
                    );
                    break;

                case 1:
                    pensamentos.Add(
                        "Sua saúde financeira está razoável."
                    );
                    break;

                default:
                    pensamentos.Add(
                        "Sua saúde financeira está excelente."
                    );
                    break;
            }

            return pensamentos;
        }

        public List<ReducaoModel> GerarReducoes(int id)
        {
            List<ReducaoModel> lista = new();

            using var conn =
                new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                SELECT
                    gasto_id,
                    nome,
                    vlr_min,
                    vlr_max
                FROM gastos
                WHERE user_id = @id
                AND ativo = TRUE
            ", conn);

            cmd.Parameters.AddWithValue("@id", id);

            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                float min =
                    Convert.ToSingle(rd["vlr_min"]);

                float max =
                    Convert.ToSingle(rd["vlr_max"]);

                float media = (min + max) / 2;

                lista.Add(new ReducaoModel
                {
                    GastoId = rd["gasto_id"].ToString(),
                    Nome = rd["nome"].ToString(),
                    ValorAtual = media,
                    ValorSugerido = media * 0.8f
                });
            }

            return lista;
        }

        public List<ExclusaoModel> GerarExclusoes(int id)
        {
            List<ExclusaoModel> lista = new();

            using var conn =
                new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                SELECT
                    id_gasto,
                    nome,
                    vlr_min,
                    vlr_max
                FROM gastos
                WHERE user_id = @id
                AND ativo = TRUE
                AND prioridade >= 4
            ", conn);

            cmd.Parameters.AddWithValue("@id", id);

            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                float min =
                    Convert.ToSingle(rd["vlr_min"]);

                float max =
                    Convert.ToSingle(rd["vlr_max"]);

                lista.Add(new ExclusaoModel
                {
                    GastoId = rd["id_gasto"].ToString(),
                    Nome = rd["nome"].ToString(),
                    ValorAtual = (min + max) / 2
                });
            }

            return lista;
        }
    }
}
