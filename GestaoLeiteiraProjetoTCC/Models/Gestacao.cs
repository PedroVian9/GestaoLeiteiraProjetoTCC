using SQLite;
using System;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Gestacao : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int VacaId { get; set; }
        public int? TouroId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Status { get; set; } = "Ativa";
        public int? CriaId { get; set; }
        public string Observacoes { get; set; }
        [Ignore]
        public Animal Vaca { get; set; }
        [Ignore]
        public Animal Touro { get; set; }
    }
}