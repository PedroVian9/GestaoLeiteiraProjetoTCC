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

        public async Task<List<Animal>> ObterAnimaisValidosLactacaoDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Animal>()
                .Where(a =>
                    a.PropriedadeId == propriedadeId &&
                    a.Sexo == "Feminino" &&
                    a.CategoriaAnimal == "Vaca" &&
                    a.Lactante == false &&
                    a.Status == "Ativo")
                .ToListAsync();
        }
        public async Task<List<Animal>> ObterAnimaisQueTiveramLactacaoDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var animaisDaPropriedade = await db.Table<Animal>()
                .Where(a => a.PropriedadeId == propriedadeId)
                .ToListAsync();

            var idsAnimaisPropriedade = animaisDaPropriedade.Select(a => a.Id).ToList();

            var lactacoes = await db.Table<Lactacao>()
                .Where(l => idsAnimaisPropriedade.Contains(l.AnimalId))
                .ToListAsync();

            var animaisComLactacao = lactacoes
                .Select(l => l.AnimalId)
                .Distinct()
                .ToList();

            return animaisDaPropriedade
                .Where(a => animaisComLactacao.Contains(a.Id))
                .OrderBy(a => a.NomeAnimal)
                .ToList();
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
