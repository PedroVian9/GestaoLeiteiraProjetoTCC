using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Animal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "A raça do animal é obrigatória.")]
        public int RacaId { get; set; }

        [Ignore] // Não mapear a navegação automaticamente para o objeto Raca
        public Raca Raca { get; set; }

        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        public string NomeAnimal { get; set; }

        [Required(ErrorMessage = "O número identificador é obrigatório.")]
        public string NumeroIdentificador { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string Sexo { get; set; } // Macho ou Fêmea

        [Required(ErrorMessage = "A categoria do animal é obrigatória.")]
        public string CategoriaAnimal { get; set; } // Bezerro, Novilha, Vaca, Touro

        public bool Lactante { get; set; } = false;

        // ***** ALTERAÇÃO PRINCIPAL *****
        // Esta propriedade agora é para controle de tela (UI) e lógica de negócio.
        // O atributo [Ignore] impede que o SQLite crie uma coluna "Prenha" no banco.
        // O valor desta propriedade será definido pela sua camada de serviço/lógica.
        [Ignore]
        public bool Prenha { get; set; } = false;

        [Required(ErrorMessage = "O status do animal é obrigatório.")]
        public string Status { get; set; } = "Ativo"; // Ativo, Inativo, Baixa

        public int? MaeId { get; set; }
        public int? PaiId { get; set; }

        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }
    }
}