using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Raca : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da raca e obrigatorio.")]
        public string NomeRaca { get; set; }

        [Required]
        public string Status { get; set; } = "Usuario";

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
