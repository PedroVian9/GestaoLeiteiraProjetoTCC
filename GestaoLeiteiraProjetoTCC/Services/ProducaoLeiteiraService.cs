using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class ProducaoLeiteiraService : IProducaoLeiteiraService
    {
        private readonly IProducaoLeiteiraRepository _producaoLeiteiraRepository;

        public ProducaoLeiteiraService(IProducaoLeiteiraRepository producaoLeiteiraRepository)
        {
            _producaoLeiteiraRepository = producaoLeiteiraRepository;
        }

        public async Task<int> CriarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira)
        {
            return await _producaoLeiteiraRepository.CriarProducaoLeiteiraDb(producaoLeiteira);
        }

        public async Task AtualizarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira)
        {
            await _producaoLeiteiraRepository.AtualizarProducaoLeiteiraDb(producaoLeiteira);
        }

        public async Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoAsync(int lactacaoId)
        {
            return await _producaoLeiteiraRepository.ObterPorLactacaoAsync(lactacaoId);
        }

        public async Task<List<ProducaoLeiteira>> ObterPorPropriedadeAsync(int propriedadeId)
        {
            return await _producaoLeiteiraRepository.ObterPorPropriedadeAsync(propriedadeId);
        }

        public async Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoNoDiaAsync(int lactacaoId, DateTime dia)
        {
            return await _producaoLeiteiraRepository.ObterProducoesPorLactacaoNoDiaAsync(lactacaoId, dia);
        }
    }
}