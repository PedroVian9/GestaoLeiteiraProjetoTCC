﻿using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class LactacaoService : ILactacaoService
    {
        private readonly ILactacaoRepository _lactacaoRepository;

        public LactacaoService(ILactacaoRepository lactacaoRepository)
        {
            _lactacaoRepository = lactacaoRepository;
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
            if (lactacao != null)
            {
                lactacao.DataFim = DateTime.Now;
                await _lactacaoRepository.AtualizarLactacaoDb(lactacao);
            }
        }

        public bool PodeEditarLactacao(int id)
        {
            var lactacao = _lactacaoRepository.ObterLactacaoPorIdDb(id).Result;
            return lactacao != null && lactacao.DataFim == null;
        }
    }
}