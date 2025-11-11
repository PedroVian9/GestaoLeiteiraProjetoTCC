using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface ILactacaoRepository
    {
        Task<int> CriarLactacaoDb(Lactacao lactacao);
        Task<List<Lactacao>> ObterLactacoesPorAnimalDb(int animalId);
        Task<Lactacao> ObterLactacaoPorIdDb(int id);
        Task AtualizarLactacaoDb(Lactacao lactacao);
        Task<List<Lactacao>> ObterTodasLactacoesAtivasDb(int propriedadeId);
        Task<List<Lactacao>> ObterLactacoesDaPropriedadeDb(int propriedadeId);
    }
}
