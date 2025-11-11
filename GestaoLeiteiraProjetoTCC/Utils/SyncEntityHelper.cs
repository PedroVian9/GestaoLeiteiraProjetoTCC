using GestaoLeiteiraProjetoTCC.Models;
using System;

namespace GestaoLeiteiraProjetoTCC.Utils
{
    public static class SyncEntityHelper
    {
        public static void Touch(ISyncEntity entity, string deviceId)
        {
            if (entity == null)
            {
                return;
            }

            if (entity.SyncId == Guid.Empty)
            {
                entity.SyncId = Guid.NewGuid();
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.LastChangedByDevice = deviceId;
        }

        public static void MarkDeleted(ISyncEntity entity, string deviceId)
        {
            if (entity == null)
            {
                return;
            }

            Touch(entity, deviceId);
            entity.IsDeleted = true;
        }
    }
}
