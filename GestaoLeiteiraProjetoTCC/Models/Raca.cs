using SQLite;
using System.ComponentModel.DataAnnotations;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class Raca : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        public string NomeRaca { get; set; }

        [Required]
        public string Status { get; set; } = "Usuario"; 
    }
}