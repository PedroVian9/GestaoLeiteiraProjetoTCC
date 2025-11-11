using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Lactacao : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O animal e obrigatorio.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "A data de inicio e obrigatoria.")]
        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        [Required(ErrorMessage = "A propriedade e obrigatoria.")]
        public int PropriedadeId { get; set; }

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
