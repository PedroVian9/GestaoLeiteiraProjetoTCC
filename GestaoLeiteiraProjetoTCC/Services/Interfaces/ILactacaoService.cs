using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ILactacaoService
    {
        Task<int> CriarLactacaoAsync(Lactacao lactacao);

        Task<List<Lactacao>> ObterLactacoesPorAnimalAsync(int animalId);

        Task FinalizarLactacaoAsync(int id);

        bool PodeEditarLactacao(int id);
    }
}