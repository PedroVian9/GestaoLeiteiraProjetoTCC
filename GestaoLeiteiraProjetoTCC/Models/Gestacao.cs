using SQLite;
using System;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Gestacao : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int VacaId { get; set; }

        public int? TouroId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataConfirmacao { get; set; }

        public DateTime? DataFim { get; set; }

        public string TipoCobertura { get; set; }

        public string Status { get; set; }

        public double? ScoreCorporal { get; set; }

        public int? CriaId { get; set; }

        public string Observacoes { get; set; }

        [Ignore]
        public Animal Vaca { get; set; }

        [Ignore]
        public Animal Touro { get; set; }

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
