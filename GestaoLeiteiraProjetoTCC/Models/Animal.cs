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

        public string Sexo { get; set; } // Masculino ou Feminino

        [Required(ErrorMessage = "A categoria do animal é obrigatória.")]
        public string CategoriaAnimal { get; set; } // Bezerro, Novilha, Vaca (em lactação ou não), Touro

        public string EstadoFisiologico { get; set; } // Lactante, Seca, Prenha (apenas se for fêmea)

        [Required(ErrorMessage = "O status do animal é obrigatório.")]
        public string Status { get; set; } // Ativo, Inativo, Baixa

        // Relacionamento com os pais (Auto-relacionamento)
        public int? MaeId { get; set; }
        public int? PaiId { get; set; }

        // Relacionamento com a propriedade
        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }
    }
}