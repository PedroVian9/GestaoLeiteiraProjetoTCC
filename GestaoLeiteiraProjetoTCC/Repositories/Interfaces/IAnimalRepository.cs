using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IAnimalRepository
    {
        Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId);
        Task<List<Animal>> ObterAnimaisValidosLactacaoDb(int propriedadeId);
        Task<List<Animal>> ObterAnimaisQueTiveramLactacaoDb(int propriedadeId);
        Task<Animal> CadastrarAnimalDb(Animal animal);
        Task<Animal> AtualizarAnimalDb(Animal animal);
        Task<bool> ExcluirAnimalDb(int id);
    }
}
