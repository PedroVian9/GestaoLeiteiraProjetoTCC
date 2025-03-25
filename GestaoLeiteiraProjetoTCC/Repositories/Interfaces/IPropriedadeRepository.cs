using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IPropriedadeRepository
    {
        Task<Propriedade> CadastrarPropriedadeDb(Propriedade propriedade);
        Task<List<Propriedade>> ObterTodasPropriedadesDb();
        Task<Propriedade> ObterPropriedadePorIdDb(int id);
        Task<Propriedade> ValidarLoginDb(string nomeProprietario, string senha);
        Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId);
    }
}
