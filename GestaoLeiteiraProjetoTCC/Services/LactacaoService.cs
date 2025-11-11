using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var lactacao = await _lactacaoRepository.ObterLactacaoPorIdDb(id);

            if (lactacao != null && lactacao.DataFim == null)
            {
                lactacao.DataFim = DateTime.Now;
                await _lactacaoRepository.AtualizarLactacaoDb(lactacao);

                var animal = await _animalService.ObterAnimalPorIdAsync(lactacao.AnimalId);
                if (animal != null)
                {
                    animal.Lactante = false;
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
