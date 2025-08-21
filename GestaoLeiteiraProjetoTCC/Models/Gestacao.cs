using SQLite;
using System;

namespace GestaoLeiteiraProjetoTCC.Models
{
    // A única definição da classe Gestacao estará neste arquivo.
    public class Gestacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Chave estrangeira para o animal (a mãe). Essencial.
        public int VacaId { get; set; }

        // Chave estrangeira para o animal (o pai). Opcional.
        public int? TouroId { get; set; }

        // Data em que a gestação foi confirmada/iniciada.
        public DateTime DataInicio { get; set; }

        // Data do parto ou fim da gestação. Nula enquanto estiver ativa.
        public DateTime? DataFim { get; set; }

        // Status para controle do ciclo: "Ativa", "Finalizada - Parto", "Finalizada - Aborto".
        public string Status { get; set; } = "Ativa";

        // Chave estrangeira para a cria que nasceu desta gestação.
        public int? CriaId { get; set; }

        // Campo opcional para anotações sobre a gestação.
        public string Observacoes { get; set; }

        [Ignore]
        public Animal Vaca { get; set; }

        [Ignore]
        public Animal Touro { get; set; }
    }
}