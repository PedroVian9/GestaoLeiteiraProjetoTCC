using GestaoLeiteiraProjetoTCC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IProducaoLeiteiraService
    {
        Task<int> CriarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira);
        Task AtualizarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira);
        Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoAsync(int lactacaoId);
        Task<List<ProducaoLeiteira>> ObterPorPropriedadeAsync(int propriedadeId);
    }
}