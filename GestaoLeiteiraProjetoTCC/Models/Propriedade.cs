using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Propriedade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do proprietário é obrigatório")]
        public string NomeProprietario { get; set; }

        [Required(ErrorMessage = "O nome da propriedade é obrigatório")]
        public string NomePropriedade { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória")]
        public string Localizacao { get; set; }

        [Required(ErrorMessage = "A área total é obrigatória")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A área total deve ser maior que zero")]
        public double AreaTotal { get; set; }

        public int TipoUnidade { get; set; } 

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}