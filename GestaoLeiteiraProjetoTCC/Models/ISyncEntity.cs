using System;

namespace GestaoLeiteiraProjetoTCC.Models
{
    public interface ISyncEntity
    {
        int Id { get; set; }
        Guid SyncId { get; set; }
        DateTime UpdatedAt { get; set; }
        bool IsDeleted { get; set; }
        string LastChangedByDevice { get; set; }
    }
}
