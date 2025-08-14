using GestaoLeiteiraProjetoTCC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IProducaoLeiteiraRepository
    {
        Task<int> CriarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira);
        Task AtualizarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira);
        Task<List<ProducaoLeiteira>> ObterPorLactacaoAsync(int lactacaoId);
        Task<List<ProducaoLeiteira>> ObterPorPropriedadeDb(int propriedadeId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoNoDiaAsync(int lactacaoId, DateTime dia);
    }
}