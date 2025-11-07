using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using erp_pessoal.Models;
using System.Security.Cryptography.X509Certificates;
namespace erp_pessoal.Controllers
{
    public class ThinkingUtils
    {
        #region Indicadores
        public float IndiceEndividamento(string id)
        {

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var rendas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM rendas WHERE user_id = @id AND ativo = TRUE;", conn);
            var dividas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM divida WHERE user_id = @id AND ativo = TRUE;", conn);
            dividas_cmd.Parameters.AddWithValue("@id", id);
            rendas_cmd.Parameters.AddWithValue("@id", id);

            var dividas = dividas_cmd.ExecuteReader();
            var rendas = rendas_cmd.ExecuteReader();
            return (float)dividas["sum"] / (float)rendas["sum"] * 100;
        }
        public RelacaoGastosModel RelacaoGastos(string id)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var rendas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM rendas WHERE user_id = @id AND ativo = TRUE;", conn);
            var gastos_fix_cmd = new NpgsqlCommand(@"SELECT SUM(valor_min) AS min, SUM(valor_max) AS max
            FROM GASTOS 
            WHERE user_id = @id AND ativo = TRUE AND fixvar = 'fix';");

            var gastos_var_cmd = new NpgsqlCommand(@"SELECT SUM(valor_min) AS min, SUM(valor_max) AS max
            FROM GASTOS 
            WHERE user_id = @id AND ativo = TRUE AND fixvar = 'var';");

            RelacaoGastosModel relacao = new RelacaoGastosModel();
            gastos_fix_cmd.Parameters.AddWithValue("@id", id);
            gastos_var_cmd.Parameters.AddWithValue("@id", id);
            rendas_cmd.Parameters.AddWithValue("@id", id);

            var gastos_fix = gastos_fix_cmd.ExecuteReader();
            var gastos_var = gastos_var_cmd.ExecuteReader();
            var rendas = rendas_cmd.ExecuteReader();

            float media_fix = ((float)gastos_fix["min"] + (float)gastos_fix["max"]) / 2;
            float media_var = ((float)gastos_var["min"] + (float)gastos_var["max"]) / 2;

            relacao.PorcentagemGastosFixos = media_fix / (float)rendas["sum"] * 100;
            relacao.PorcentagemGastosVariaveis = media_var / (float)rendas["sum"] * 100;

            return relacao;
        }

        public string GastoMensal(string dataInicio, string dataFim, string id)
        {
            int recomendacao_1 = 0;
            int recomendacao_2 = 0;

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var pgto_cmd = new NpgsqlCommand(@"SELECT SUM(valor_min) AS total FROM pagamentos WHERE 
               user_id = @id AND ativo = TRUE AND data >= @datainicio AND data <= @datafim;");
            var gastos_cmd = new NpgsqlCommand(@"SELECT SUM(valor_min) AS min, SUM(valor_max) AS max
            FROM GASTOS 
            WHERE user_id = @id AND ativo = TRUE;");

            pgto_cmd.Parameters.AddWithValue("@id", id);
            pgto_cmd.Parameters.AddWithValue("@datainicio", DateTime.Parse(dataInicio));
            pgto_cmd.Parameters.AddWithValue("@datafim", DateTime.Parse(dataFim));
            gastos_cmd.Parameters.AddWithValue("@id", id);

            var gastos = gastos_cmd.ExecuteReader();
            var pgto = pgto_cmd.ExecuteReader();

            float indice = (float)pgto["total"] / (float)gastos["max"];
            float indice_media = (float)pgto["total"] / (((float)gastos["max"] + (float)gastos["min"]) / 2);

            if (indice >= 1)
            {
                recomendacao_1 = -1;
            }
            else if (indice < 1)
            {
                recomendacao_1 = 1;
            }
            if (indice_media >= 1)
            {
                recomendacao_2 = -1;
            }
            else if (indice_media < 1)
            {
                recomendacao_2 = 1;
            }
            switch (recomendacao_1 + recomendacao_2)
            {
                case 2:
                    return "bom";
                case 0:
                    return "atenção";
                case -2:
                    return "ruim";
                default:
                    return "indefinido";
            }
        }

        public SaudeRendaModel SaudeRenda(string id)
        {
            int recomenda;

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var gastos_cmd = new NpgsqlCommand(@"SELECT SUM(valor_min) AS min, SUM(valor_max) AS max
            FROM GASTOS 
            WHERE user_id = @id AND ativo = TRUE;");
            var rendas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM rendas WHERE user_id = @id AND ativo = TRUE;", conn);

            gastos_cmd.Parameters.AddWithValue("@id", id);
            rendas_cmd.Parameters.AddWithValue("@id", id);

            var gastos = gastos_cmd.ExecuteReader();
            var rendas = rendas_cmd.ExecuteReader();

            float indice = ((((float)gastos["max"] + (float)gastos["min"]) / 2) / (float)rendas["sum"]) * 100;

            if (indice >= 70 && indice < 100)
            {
                recomenda = 2;
            }
            else if (indice >= 100)
            {
                recomenda = 3;
            }
            else if (indice < 70 && indice > 55)
            {
                recomenda = 1;
            }
            else
            {
                recomenda = 0;
            }

            return new SaudeRendaModel
            {
                IndiceSaudeFinanceira = (float)Math.Round(indice, 2),
                Recomendacoes = recomenda
            };
        }
        #endregion

        #region Previsoes
        public PrevisaoModel PreverQuitacao(string id, string divida_id)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            var dividas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM divida WHERE user_id = @id AND ativo = TRUE;", conn);
            var pgto_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM pgto_divida WHERE divida_id = @id AND ativo = TRUE;", conn);
            var pgto_3_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM pgto_divida WHERE divida_id = @id AND data >= @data_inicio AND data <= @data_fim AND ativo = TRUE;", conn);

            dividas_cmd.Parameters.AddWithValue("@id", id);
            pgto_cmd.Parameters.AddWithValue("@id", divida_id);
            pgto_3_cmd.Parameters.AddWithValue("@id", divida_id);
            pgto_3_cmd.Parameters.AddWithValue("@data_inicio", DateTime.Now.AddMonths(-3));
            pgto_3_cmd.Parameters.AddWithValue("@data_fim", DateTime.Now);

            var dividas = dividas_cmd.ExecuteReader();
            var pgtos_all = pgto_cmd.ExecuteReader();
            var pgtos_3 = pgto_3_cmd.ExecuteReader();

            float expectativaPgto = ((float)dividas["sum"] - (float)pgtos_all["sum"]) / (((float)pgtos_3["sum"]) / 3);
            float valorFaltante = (float)dividas["sum"] - (float)pgtos_all["sum"];
            int mesInteiro = (int)expectativaPgto;
            int diasExtras = (int)((expectativaPgto - mesInteiro) * 30);
            DateTime previsao = DateTime.Now.AddMonths(mesInteiro).AddDays(diasExtras);
            return new PrevisaoModel
            {
                DataPrevista = previsao,
                ValorFaltante = (float)Math.Round(valorFaltante, 2),
                Progresso = (float)Math.Round(((float)pgtos_all["sum"] / (float)dividas["sum"]) * 100, 2)
            };
        }

        public PrevisaoModel PrevisaoMeta(string id, string meta_id)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            var dividas_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM meta WHERE user_id = @id AND ativo = TRUE;", conn);
            var pgto_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM meta_pgto WHERE meta_id = @id AND ativo = TRUE;", conn);
            var pgto_3_cmd = new NpgsqlCommand("SELECT SUM(valor) FROM meta_pgto WHERE meta_id = @id AND data >= @data_inicio AND data <= @data_fim AND ativo = TRUE;", conn);

            dividas_cmd.Parameters.AddWithValue("@id", id);
            pgto_cmd.Parameters.AddWithValue("@id", meta_id);
            pgto_3_cmd.Parameters.AddWithValue("@id", meta_id);
            pgto_3_cmd.Parameters.AddWithValue("@data_inicio", DateTime.Now.AddMonths(-3));
            pgto_3_cmd.Parameters.AddWithValue("@data_fim", DateTime.Now);

            var dividas = dividas_cmd.ExecuteReader();
            var pgtos_all = pgto_cmd.ExecuteReader();
            var pgtos_3 = pgto_3_cmd.ExecuteReader();

            float expectativaPgto = ((float)dividas["sum"] - (float)pgtos_all["sum"]) / (((float)pgtos_3["sum"]) / 3);
            float valorFaltante = (float)dividas["sum"] - (float)pgtos_all["sum"];
            int mesInteiro = (int)expectativaPgto;
            int diasExtras = (int)((expectativaPgto - mesInteiro) * 30);
            DateTime previsao = DateTime.Now.AddMonths(mesInteiro).AddDays(diasExtras);
            return new PrevisaoModel
            {
                DataPrevista = previsao,
                ValorFaltante = (float)Math.Round(valorFaltante, 2),
                Progresso = (float)Math.Round(((float)pgtos_all["sum"] / (float)dividas["sum"]) * 100, 2)
            };
        }
        #endregion

        public string GerarReducoes(string id, int Referencia)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var sql = @"
            SELECT g.gasto_id, g.nome, g.vlr_min, g.vlr_max, g.prioridade
            FROM gastos g
            WHERE g.user_id = @id
                AND NOT EXISTS (
                    SELECT 1
                    FROM restricoes_usuario r
                    WHERE r.user_id = g.user_id
                        AND r.gasto_id = g.gasto_id
                        AND r.bloqueado = TRUE
                  )
            ORDER BY g.prioridade ASC, (g.vlr_min + g.vlr_max) / 2 DESC
            LIMIT 1; -- já traz só a melhor sugestão";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            var sugest = new SugestoesModel();

            using (var rd = cmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    sugest.GastoId = rd.GetString(0);
                    sugest.GastoNome = rd.GetString(1);
                    sugest.VlrMin = rd.GetFloat(2);
                    sugest.VlrMax = rd.GetFloat(2);
                    sugest.Prioridade = rd.GetInt32(4);
                }
            }
            var ValidaReducao_cmd = new NpgsqlCommand(@"SELECT excluir, reduzir
            FROM restricoes_usuario
            WHERE user_id = % s AND gasto_id = % s AND bloqueado = FALSE;");

            ValidaReducao_cmd.Parameters.AddWithValue("@id", id);
            ValidaReducao_cmd.Parameters.AddWithValue("@gasto_id", sugest.GastoId);

            var ValidaReducao = ValidaReducao_cmd.ExecuteReader();

            if (sugest.Prioridade <= Referencia)
            {
                if (ValidaReducao["excluir"] != null)
                {
                    return "Já está planejada a exclusão do Gasto" + sugest.GastoNome;
                }
                else if (ValidaReducao["reduzir"] != null)
                {
                    return "Já está planejada a redução de " + sugest.GastoNome + " para R$ " + ((sugest.VlrMin + sugest.VlrMax) / 2 * 80 / 100);
                }
                else
                {
                    return "Recomenda-se reduzir o gasto " + sugest.GastoNome + " para R$ " + ((sugest.VlrMin + sugest.VlrMax) / 2 * 80 / 100);
                }
            }
            else
            {
                return "Seus gastos estão dentro do esperado.";
            }
        }

        public Tuple<EstruturaSugestaoModel, IndicadoresModel> GerarSugestoes(string id_user)
        {
            EstruturaSugestaoModel resultado = new EstruturaSugestaoModel();
            IndicadoresModel indicadores = new IndicadoresModel();
            int reducao= 0;
            IEnumerable<string> previsaoDivida = new List<string>();
            IEnumerable<string> previsaoMeta = new List<string>();

            //Definir indicadores

            if (indicadores.Endividamento >= 50)
            {
                resultado.Endividamento = "Seu índice de endividamento está alto. Priorize pagar dívidas antes de novos gastos.";
            } else if(indicadores.Endividamento > 30 && indicadores.Endividamento < 50)
            {
                resultado.Endividamento = "Seu endividamento está moderado. Mantenha o controle para evitar problemas futuros.";
            }
            else if (indicadores.Endividamento < 30)
            {
                resultado.Endividamento = "Seu endividamento está equilibrado. Pode adquirir dívidas com segurança, desde que de forma moderada.";
            }

            if (indicadores.Gastos.PorcentagemGastosFixos > 70)
            {
                resultado.GastosFixos = "Seus gastos fixos estão altos. Tente renegociar contratos ou reduzir assinaturas.";
            } else
            {
                resultado.GastosFixos = "Seus gastos fixos estão dentro do esperado. Mantenha o controle.";
            }

            if (indicadores.Gastos.PorcentagemGastosVariaveis > 30)
            {
                resultado.GastosVariaveis = "Seus gastos variáveis são elevados. Avalie onde pode economizar no dia a dia.";
            } else
            {
                resultado.GastosVariaveis = "Seus gastos variáveis estão dentro do esperado. Mantenha o controle.";
            }

            switch (indicadores.IndiceSaude.Recomendacoes)
            {
                case 3:
                    resultado.IndiceSaude = "Sua saúde financeira está comprometida. Revise suas despesas urgentemente.";
                    reducao = 5;
                    break;
                case 2:
                    resultado.IndiceSaude =  "Atenção! Seus gastos estão próximos do limite saudável da sua renda.";
                    reducao = 4;
                    break;
                case 1:
                    resultado.IndiceSaude = "Seus gastos estão saudáveis, pode trabalhar confortávelmente com essa margem";
                    break;
                case 0:
                    resultado.IndiceSaude = "Sua saúde financeira está perfeita, pode destinar esse dinheiro para investimentos e lazer, confortavelmente";
                    break;
            }

            switch (indicadores.AvaliacaoGastoMensal.ToLower())
            {
                case "bom":
                    resultado.GastoMensal = "Parabéns! Seu controle financeiro está adequado este mês.";
                    break;
                case "atenção":
                    resultado.GastoMensal = "Você está gastando perto do seu limite. Redobre a atenção nos próximos meses.";
                    break;
                case "ruim":
                    resultado.GastoMensal = "Seus gastos foram muito altos este mês. É essencial rever seu orçamento.";
                    break;
            }

            foreach (var divida in indicadores.PrevisaoDivida)
            {
                if (divida.ValorFaltante > 5000) 
                {
                    previsaoDivida.Append($"Você ainda deve R${divida.ValorFaltante:N2}. Tente aumentar o pagamento mensal para reduzir juros.");
                }
            }

            foreach (var meta in indicadores.PrevisaoMeta)
            {
                if (meta.Progresso < 50) 
                {
                    previsaoMeta.Append($"Seu progresso na meta está em {meta.Progresso:N2}%. Tente contribuir mais para alcançar seus objetivos.");
                }
            }

            resultado.PrevisaoDivida = previsaoDivida;
            resultado.PrevisaoMeta = previsaoMeta;

            if (reducao != 0)
            {
                resultado.Reducao = GerarReducoes(id_user, reducao);
            }

            return Tuple.Create(resultado, indicadores);
        }
    }
}
