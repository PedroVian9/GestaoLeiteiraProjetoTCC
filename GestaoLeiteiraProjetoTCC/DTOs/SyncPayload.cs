using GestaoLeiteiraProjetoTCC.Models;
using System;
using System.Collections.Generic;

namespace GestaoLeiteiraProjetoTCC.DTOs
{
    public class SyncEnvelope
    {
        public string SourceDeviceId { get; set; }
        public string SourceDeviceName { get; set; }
        public string SourcePlatform { get; set; }
        public DateTime ExportedAtUtc { get; set; }
        public SyncData Data { get; set; } = new();
    }

    public class SyncData
    {
        public List<Propriedade> Propriedades { get; set; } = new();
        public List<Raca> Racas { get; set; } = new();
        public List<Animal> Animais { get; set; } = new();
        public List<Lactacao> Lactacoes { get; set; } = new();
        public List<ProducaoLeiteira> Producoes { get; set; } = new();
        public List<Gestacao> Gestacoes { get; set; } = new();
        public List<QuantidadeOrdenha> QuantidadesOrdenha { get; set; } = new();
    }
}
