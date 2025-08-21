using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IPropriedadeService _propriedadeService;

        public AnimalService(IAnimalRepository animalRepository, IPropriedadeService propriedadeService)
        {
            _animalRepository = animalRepository;
            _propriedadeService = propriedadeService;
        }

        public async Task<List<Animal>> ObterAnimaisDaPropriedadeAsync(int propriedadeId)
        {

            return await _animalRepository.ObterAnimaisPorPropriedadeIdDb(propriedadeId);
        }

        public async Task<Animal> ObterAnimalPorIdAsync(int id)
        {
            return await _animalRepository.ObterAnimalPorIdDb(id);
        }

        public async Task<List<Animal>> ObterAnimaisValidosLactacao(int propriedadeId)
        {
            return await _animalRepository.ObterAnimaisValidosLactacaoDb(propriedadeId);
        }

        public async Task<List<Animal>> ObterAnimaisQueTiveramLactacao(int propriedadeId)
        {
            return await _animalRepository.ObterAnimaisQueTiveramLactacaoDb(propriedadeId);
        }

        public async Task<Animal> CadastrarAnimalAsync(Animal animal)
        {
            var propriedadeLogada = _propriedadeService.ObterPropriedadeLogada();
            if (propriedadeLogada == null)
                throw new InvalidOperationException("Nenhuma propriedade está logada.");

            animal.PropriedadeId = propriedadeLogada.Id;
            return await _animalRepository.CadastrarAnimalDb(animal);
        }

        public async Task<Animal> AtualizarAnimalAsync(Animal animal)
        {
            return await _animalRepository.AtualizarAnimalDb(animal);
        }

        public async Task<bool> ExcluirAnimalAsync(int id)
        {
            return await _animalRepository.ExcluirAnimalDb(id);
        }

        public async Task<List<Animal>> ObterVacasAptasParaGestacao(int propriedadeId)
        {
            return await _animalRepository.ObterVacasAptasParaGestacaoDb(propriedadeId);
        }

        public async Task<List<Animal>> ObterTourosAtivos(int propriedadeId)
        {
            return await _animalRepository.ObterTourosAtivosDb(propriedadeId);
        }
    }
}
