using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Animal : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "A raça do animal é obrigatória.")]
        public int RacaId { get; set; }

        [Ignore] 
        public Raca Raca { get; set; }

        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        public string NomeAnimal { get; set; }

        [Required(ErrorMessage = "O número identificador é obrigatório.")]
        public string NumeroIdentificador { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string Sexo { get; set; }

        [Required(ErrorMessage = "A categoria do animal é obrigatória.")]
        public string CategoriaAnimal { get; set; } 

        public bool Lactante { get; set; } = false;

        [Ignore]
        public bool Prenha { get; set; } = false;

        [Required(ErrorMessage = "O status do animal é obrigatório.")]
        public string Status { get; set; } = "Ativo";

        public int? MaeId { get; set; }
        public int? PaiId { get; set; }

        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }
    }
}