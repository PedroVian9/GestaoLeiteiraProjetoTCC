using SQLite;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Animal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public string Especie { get; set; }

        public string? Raca { get; set; }

        [Required(ErrorMessage = "O nome identificador é obrigatório.")]
        public string NomeIdentificador { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string? EstadoFisiologico { get; set; } // Lactante, Seca, Prenha

        public int? MaeId { get; set; }
        public int? PaiId { get; set; }

        [ForeignKey("MaeId")]
        public Animal? Mae { get; set; }

        [ForeignKey("PaiId")]
        public Animal? Pai { get; set; }

        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }

        [ForeignKey("PropriedadeId")]
        public Propriedade Propriedade { get; set; }
    }
}