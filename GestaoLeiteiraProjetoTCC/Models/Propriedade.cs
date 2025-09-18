using SQLite;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Propriedade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do proprietário é obrigatório")]
        public string NomeProprietario { get; set; }

        // Campo opcional para nome social
        public string? NomeSocial { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        [Range(1, 3, ErrorMessage = "Selecione uma opção válida para o sexo.")]
        public int Sexo { get; set; } // 1: Masculino, 2: Feminino, 3: Não Informar

        [Required(ErrorMessage = "O nome da propriedade é obrigatório")]
        public string NomePropriedade { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória")]
        public string Localizacao { get; set; }

        [Required(ErrorMessage = "A área total é obrigatória")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A área total deve ser maior que zero")]
        public double AreaTotal { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        [Range(1, 2, ErrorMessage = "Selecione uma unidade válida.")]
        public int TipoUnidade { get; set; } // 1: Alqueire, 2: Hectare

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; }

        // Campo para confirmação de senha
        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [Ignore] // Importante: Ignora este campo ao salvar no SQLite
        public string ConfirmarSenha { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}