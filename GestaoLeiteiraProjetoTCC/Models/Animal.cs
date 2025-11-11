using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Animal : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "A raca do animal e obrigatoria.")]
        public int RacaId { get; set; }

        [Ignore]
        public Raca Raca { get; set; }

        [Required(ErrorMessage = "O nome do animal e obrigatorio.")]
        public string NomeAnimal { get; set; }

        [Required(ErrorMessage = "O numero identificador e obrigatorio.")]
        public string NumeroIdentificador { get; set; }

        public DateTime? DataNascimento { get; set; }

        public double PesoNascimento { get; set; }

        public string Sexo { get; set; }

        [Required(ErrorMessage = "A categoria do animal e obrigatoria.")]
        public string CategoriaAnimal { get; set; }

        public bool Lactante { get; set; }

        [Ignore]
        public bool Prenha { get; set; }

        [Required(ErrorMessage = "O status do animal e obrigatorio.")]
        public string Status { get; set; } = "Ativo";

        public DateTime? DataUltimoParto { get; set; }

        public int NumeroDePartos { get; set; }

        public int NumeroDeAbortos { get; set; }

        public int NumeroDeNascimortos { get; set; }

        public int? MaeId { get; set; }

        public int? PaiId { get; set; }

        [Required(ErrorMessage = "O id da propriedade e obrigatorio.")]
        public int PropriedadeId { get; set; }

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
