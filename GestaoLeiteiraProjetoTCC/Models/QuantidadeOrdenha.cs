using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class QuantidadeOrdenha : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O quantidade deve ser iformada.")]
        public int Quantidade { get; set; } = 2;

        public DateTime DataRegistro { get; set; } = DateTime.Now;
    }
}