using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class LactacaoService : ILactacaoService
    {
        private readonly ILactacaoRepository _lactacaoRepository;
        private readonly IAnimalService _animalService;

        public LactacaoService(ILactacaoRepository lactacaoRepository, IAnimalService animalService)
        {
            _lactacaoRepository = lactacaoRepository;
            _animalService = animalService;
        }

        public async Task<int> CriarLactacaoAsync(Lactacao lactacao)
        {
            return await _lactacaoRepository.CriarLactacaoDb(lactacao);
        }

        public async Task<List<Lactacao>> ObterLactacoesPorAnimalAsync(int animalId)
        {
            return await _lactacaoRepository.ObterLactacoesPorAnimalDb(animalId);
        }

        public async Task FinalizarLactacaoAsync(int id)
        {
            // 1. Busca a lactação que será finalizada pelo ID.
            var lactacao = await _lactacaoRepository.ObterLactacaoPorIdDb(id);

            // 2. Garante que a lactação existe e que ela ainda está ativa (não tem data de fim).
            //    Isso evita re-finalizar uma lactação que já foi encerrada.
            if (lactacao != null && lactacao.DataFim == null)
            {
                // 3. Define a data de fim e atualiza o registro da lactação no banco.
                lactacao.DataFim = DateTime.Now;
                await _lactacaoRepository.AtualizarLactacaoDb(lactacao);

                // 4. Busca o animal associado a esta lactação.
                var animal = await _animalService.ObterAnimalPorIdAsync(lactacao.AnimalId);
                if (animal != null)
                {
                    // 5. Atualiza o status do animal para não-lactante.
                    animal.Lactante = false;

                    // 6. Salva a alteração do animal no banco de dados.
                    await _animalService.AtualizarAnimalAsync(animal);
                }
            }
        }

        public bool PodeEditarLactacao(int id)
        {
            var lactacao = _lactacaoRepository.ObterLactacaoPorIdDb(id).Result;
            return lactacao != null && lactacao.DataFim == null;
        }

        public async Task<List<Lactacao>> ObterTodasLactacoesAtivasAsync(int propriedadeId)
        {
            return await _lactacaoRepository.ObterTodasLactacoesAtivasDb(propriedadeId);
        }

        public async Task<List<Lactacao>> ObterLactacoesDaPropriedadeAsync(int propriedadeId)
        {
            return await _lactacaoRepository.ObterLactacoesDaPropriedadeDb(propriedadeId);
        }
    }
}