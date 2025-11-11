using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Propriedade : ISyncEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do proprietario e obrigatorio.")]
        public string NomeProprietario { get; set; }

        public string? NomeSocial { get; set; }

        [Required(ErrorMessage = "O sexo e obrigatorio.")]
        [Range(1, 3, ErrorMessage = "Selecione uma opcao valida para o sexo.")]
        public int Sexo { get; set; }

        [Required(ErrorMessage = "O nome da propriedade e obrigatorio.")]
        public string NomePropriedade { get; set; }

        [Required(ErrorMessage = "A localizacao e obrigatoria.")]
        public string Localizacao { get; set; }

        [Required(ErrorMessage = "A area total e obrigatoria.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A area total deve ser maior que zero.")]
        public double AreaTotal { get; set; }

        [Required(ErrorMessage = "A unidade de medida e obrigatoria.")]
        [Range(1, 3, ErrorMessage = "Selecione uma unidade valida.")]
        public int TipoUnidade { get; set; }

        [Required(ErrorMessage = "A senha e obrigatoria.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A confirmacao de senha e obrigatoria.")]
        [Compare("Senha", ErrorMessage = "As senhas nao conferem.")]
        [Ignore]
        public string ConfirmarSenha { get; set; }

        public DateTime DataCadastro { get; set; }

        [Indexed]
        public Guid SyncId { get; set; } = Guid.NewGuid();

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string LastChangedByDevice { get; set; } = string.Empty;
    }
}
