using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using Npgsql;
using WorkerAcoes.Models;

namespace WorkerAcoes.Data
{
    public class AcoesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AcoesRepository> _logger;

        public AcoesRepository(IConfiguration configuration,
            ILogger<AcoesRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Save(HistoricoAcao historico)
        {
            using var connection =
                new NpgsqlConnection(_configuration.GetConnectionString("BaseAcoes"));
            connection.Open();

            var horario = DateTime.Now;
            var acao = new Acao();
            acao.Codigo = historico.Codigo;
            acao.Valor = historico.Valor.Value;
            acao.DataReferencia = horario;
            acao.HistLancamento = historico.Codigo + horario.ToString("yyyyMMddHHmmss");
            acao.CodCorretora = historico.CodCorretora;
            acao.NomeCorretora = historico.CodCorretora; // Simulação de falha
            //acao.NomeCorretora = historico.NomeCorretora; // Correto

            connection.Insert(acao);
            connection.Close();
        }
    }
}