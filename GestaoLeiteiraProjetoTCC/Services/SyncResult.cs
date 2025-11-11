using System;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public DateTime? RemoteExportedAtUtc { get; init; }
        public int RecordsExported { get; init; }
        public int RecordsImported { get; init; }
        public string? PayloadPath { get; init; }
    }
}
