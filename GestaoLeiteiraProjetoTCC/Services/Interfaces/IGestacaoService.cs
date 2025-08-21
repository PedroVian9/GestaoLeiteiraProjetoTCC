using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IGestacaoService
    {
        Task<List<Gestacao>> ObterGestoesAtivas(int propriedadeId);
        Task<Gestacao> IniciarGestacaoAsync(Gestacao gestacao);
        Task<Gestacao> AtualizarGestacaoAsync(Gestacao gestacao);
        Task FinalizarGestacaoAsync(int gestacaoId, Animal cria);
    }
}