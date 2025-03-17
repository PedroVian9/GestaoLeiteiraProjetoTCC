using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IPropriedadeService
    {
        Task<Propriedade> CadastrarPropriedadeAsync(Propriedade propriedade);
        Task<List<Propriedade>> ObterTodasPropriedadesAsync();
        Task<Propriedade> ObterPropriedadePorIdAsync(int id);
    }
}