using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IGestacaoService
    {
        Task<List<Gestacao>> ObterCiclosAtivos(int propriedadeId);
        Task<Gestacao> IniciarCicloAsync(Gestacao ciclo);
        Task RepetirCicloAsync(int cicloAntigoId, Gestacao novoCiclo);
        Task<Gestacao> ConfirmarGestacaoAsync(int cicloId, DateTime dataConfirmacao);
        Task FinalizarGestacaoComCriaVivaAsync(int cicloId, Animal cria);
        Task FinalizarGestacaoSemCriaVivaAsync(int cicloId, string statusFinal);
    }
}