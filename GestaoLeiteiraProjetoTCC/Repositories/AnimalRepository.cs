using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System.Linq;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public AnimalRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Animal>()
                           .Where(a => a.PropriedadeId == propriedadeId && !a.IsDeleted)
                           .ToListAsync();
        }

        public async Task<List<Animal>> ObterAnimaisValidosLactacaoDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Animal>()
                .Where(a =>
                    a.PropriedadeId == propriedadeId &&
                    a.Sexo == "F\u00EAmea" &&
                    a.CategoriaAnimal == "Vaca" &&
                    !a.Lactante &&
                    a.Status == "Ativo" &&
                    !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObterAnimaisQueTiveramLactacaoDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var animaisDaPropriedade = await db.Table<Animal>()
                .Where(a => a.PropriedadeId == propriedadeId && !a.IsDeleted)
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
            SyncEntityHelper.Touch(animal, _syncMetadataService.GetDeviceId());
            await db.InsertAsync(animal);
            return animal;
        }

        public async Task<Animal> AtualizarAnimalDb(Animal animal)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(animal, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(animal);
            return animal;
        }

        public async Task<bool> ExcluirAnimalDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            var animal = await db.Table<Animal>()
                                 .Where(a => a.Id == id)
                                 .FirstOrDefaultAsync();

            if (animal == null)
            {
                return false;
            }

            SyncEntityHelper.MarkDeleted(animal, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(animal);
            return true;
        }

        public async Task<Animal> ObterAnimalPorIdDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Animal>()
                           .Where(a => a.Id == id && !a.IsDeleted)
                           .FirstOrDefaultAsync();
        }

        public async Task<List<Animal>> ObterVacasAptasParaGestacaoDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var idsVacasComGestacaoAtiva = (await db.Table<Gestacao>()
                .Where(g => g.Status == "Ativa" && !g.IsDeleted)
                .ToListAsync())
                .Select(g => g.VacaId);

            return await db.Table<Animal>()
                .Where(a => a.PropriedadeId == propriedadeId &&
                            a.Sexo == "F\u00EAmea" &&
                            a.CategoriaAnimal == "Vaca" &&
                            a.Status == "Ativo" &&
                            !a.IsDeleted &&
                            !idsVacasComGestacaoAtiva.Contains(a.Id))
                .OrderBy(a => a.NomeAnimal)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObterTourosAtivosDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Animal>()
                .Where(a => a.PropriedadeId == propriedadeId &&
                            a.Sexo == "Macho" &&
                            a.Status == "Ativo" &&
                            !a.IsDeleted)
                .OrderBy(a => a.NomeAnimal)
                .ToListAsync();
        }
    }
}
