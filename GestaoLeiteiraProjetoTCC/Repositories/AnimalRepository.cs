using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly DatabaseService _databaseService;

        public AnimalRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Animal>().Where(a => a.PropriedadeId == propriedadeId).ToListAsync();
        }

        public async Task<Animal> CadastrarAnimalDb(Animal animal)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.InsertAsync(animal);
            return animal;
        }

        public async Task<Animal> AtualizarAnimalDb(Animal animal)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.UpdateAsync(animal);
            return animal;
        }

        public async Task<bool> ExcluirAnimalDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            var resultado = await db.DeleteAsync<Animal>(id);
            return resultado > 0;
        }
    }
}
