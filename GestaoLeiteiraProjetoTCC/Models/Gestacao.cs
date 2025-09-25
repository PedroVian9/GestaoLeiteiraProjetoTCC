using SQLite;
using System;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Gestacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int VacaId { get; set; }
        public int? TouroId { get; set; }
        public DateTime DataInicio { get; set; } // Data da Cobertura (Monta/Inseminação)
        public DateTime? DataConfirmacao { get; set; } // Data em que a gestação foi confirmada
        public DateTime? DataFim { get; set; }
        public string TipoCobertura { get; set; } // "Monta Natural" ou "Inseminação"
        public string Status { get; set; } // "Em Cobertura", "Gestação Ativa", "Finalizada - Parto", etc.
        public double? ScoreCorporal { get; set; } //Escore de Condição Corporal (1-5)
        public int? CriaId { get; set; }
        public string Observacoes { get; set; }

        [Ignore]
        public Animal Vaca { get; set; }

        [Ignore]
        public Animal Touro { get; set; }
    }
}