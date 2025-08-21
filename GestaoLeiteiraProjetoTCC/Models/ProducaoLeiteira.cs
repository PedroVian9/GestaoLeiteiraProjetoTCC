using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public class ProducaoLeiteira : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required(ErrorMessage = "O animal id é obrigatório")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "A lactação id é obrigatória")]
        public int LactacaoId { get; set; }

        [Required(ErrorMessage = "O data é obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        public double Quantidade { get; set; }

        [Required(ErrorMessage = "O id da propriedade é obrigatório.")]
        public int PropriedadeId { get; set; }
    }
}
