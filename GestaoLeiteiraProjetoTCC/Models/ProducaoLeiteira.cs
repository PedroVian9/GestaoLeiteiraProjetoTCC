using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class ProducaoLeiteira : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O animal e obrigatorio.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "A lactacao e obrigatoria.")]
        public int LactacaoId { get; set; }

        [Required(ErrorMessage = "A data e obrigatoria.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A quantidade e obrigatoria.")]
        public double Quantidade { get; set; }

        [Required(ErrorMessage = "O id da propriedade e obrigatorio.")]
        public int PropriedadeId { get; set; }

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
