using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IGestacaoRepository
    {
        Task<List<Gestacao>> ObterGestoesAtivasPorPropriedadeDb(List<int> idsAnimais);
        Task<Gestacao> IniciarGestacaoDb(Gestacao gestacao);
        Task<Gestacao> AtualizarGestacaoDb(Gestacao gestacao);
        Task<Gestacao> ObterGestacaoPorIdDb(int gestacaoId);
    }
}