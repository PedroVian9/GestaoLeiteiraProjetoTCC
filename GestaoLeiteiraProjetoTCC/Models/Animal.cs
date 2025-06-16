using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Animal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "A raça é obrigatória.")]
        public string Raca { get; set; }

        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        public string NomeAnimal { get; set; }

        [Required(ErrorMessage = "O número identificador é obrigatório.")]
        public string NumeroIdentificador { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string Sexo { get; set; } // Macho ou Fêmea

        [Required(ErrorMessage = "A categoria do animal é obrigatória.")]
        public string CategoriaAnimal { get; set; } // Bezerro, Novilha, Vaca, Touro

        public bool Lactante { get; set; } = false;
        public bool Prenha { get; set; } = false;

        [Required(ErrorMessage = "O status do animal é obrigatório.")]
        public string Status { get; set; } = "Ativo"; // Ativo, Inativo, Baixa

        public int? MaeId { get; set; }
        public int? PaiId { get; set; }

        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }
    }
}