using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;
using Confluent.Kafka;
using Dapper;
using Npgsql;
using Serilog;
using Serilog.Core;
using WorkerAcoes.IntegrationTests.Data;
using WorkerAcoes.IntegrationTests.Models;

namespace WorkerAcoes.IntegrationTests
{
    public class TestesIntegracaoWorkerAcoes
    {
        private const string COD_CORRETORA = "00000";
        private const string NOME_CORRETORA = "Corretora Testes";
        private static IConfiguration Configuration { get; }
        private static Logger Logger { get; }

        static TestesIntegracaoWorkerAcoes()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables().Build();

            using var connection = new NpgsqlConnection(
                Configuration.GetConnectionString("BaseAcoes"));
            connection.Execute("TRUNCATE \"Acoes\"");

            Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Theory]
        [InlineData("ABCD", 100.98)]
        [InlineData("EFGH", 200.9)]
        [InlineData("IJKL", 1_400.97)]
        public void TestarWorkerService(string codigo, double valor)
        {
            var broker = Configuration["ApacheKafka:Broker"];
            Logger.Information($"Broker Kafka: {broker}");

            var topic = Configuration["ApacheKafka:Topic"];
            Logger.Information($"Tópico: {topic}");

            var cotacaoAcao = new Acao()
            {
                Codigo = codigo,
                Valor = valor,
                CodCorretora = COD_CORRETORA,
                NomeCorretora = NOME_CORRETORA
            };
            var conteudoAcao = JsonSerializer.Serialize(cotacaoAcao);
            Logger.Information($"Dados: {conteudoAcao}");

            var configKafka = new ProducerConfig
            {
                BootstrapServers = broker
            };

            using (var producer = new ProducerBuilder<Null, string>(configKafka).Build())
            {
                var result = producer.ProduceAsync(
                    topic,
                    new Message<Null, string>
                    { Value = conteudoAcao }).Result;

                Logger.Information(
                    $"Apache Kafka - Envio para o tópico {topic} concluído | " +
                    $"{conteudoAcao} | Status: { result.Status.ToString()}");
            }
            
            Logger.Information("Aguardando o processamento do Worker...");
            Thread.Sleep(
                Convert.ToInt32(Configuration["IntervaloProcessamento"]));

            var baseAcoesConnection = Configuration.GetConnectionString("BaseAcoes");
            Logger.Information($"BaseAcoes Connection: {baseAcoesConnection}");

            var connection = new NpgsqlConnection(baseAcoesConnection);
            connection.Open();
            HistoricoAcao historicoAcao = connection.QueryFirstOrDefault<HistoricoAcao>(
                "SELECT * FROM \"Acoes\" WHERE \"Codigo\" = @codigoAcao",
                new { codigoAcao = codigo });
            connection.Close();

            historicoAcao.Should().NotBeNull();
            historicoAcao.Codigo.Should().Be(codigo);
            historicoAcao.Valor.Should().Be(valor);
            historicoAcao.CodCorretora.Should().Be(COD_CORRETORA);
            historicoAcao.NomeCorretora.Should().Be(NOME_CORRETORA);
            historicoAcao.HistLancamento.Should().NotBeNullOrWhiteSpace();
            historicoAcao.DataReferencia.Should().NotBeNull();
        }
    }
}