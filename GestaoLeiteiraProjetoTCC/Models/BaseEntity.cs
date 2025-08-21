using SQLite;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public abstract class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Identificador global único para sincronização
        [Indexed]
        public string GuidRegistro { get; set; } = Guid.NewGuid().ToString();
        public bool Excluido { get; set; } = false;
        // Última modificação em UTC
        public DateTime DataModificacaoUtc { get; set; } = DateTime.UtcNow;
    }

}