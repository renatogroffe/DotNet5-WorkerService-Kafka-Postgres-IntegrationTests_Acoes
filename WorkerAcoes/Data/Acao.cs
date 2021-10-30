using System;
using Dapper.Contrib.Extensions;

namespace WorkerAcoes.Data
{
    [Table("\"Acoes\"")]
    public class Acao
    {
        public string HistLancamento { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public DateTime DataReferencia { get; set; }
        public string CodCorretora { get; set; }
        public string NomeCorretora { get; set; }
    }
}