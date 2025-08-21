using GestaoLeiteiraProjetoTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Lactacao : BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required(ErrorMessage = "O animal id é obrigatório")]
    public int AnimalId { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória")]
    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }
    [Required(ErrorMessage = "A propriedade Id é obrigatoria")]
    public int PropriedadeId { get; set; }
}