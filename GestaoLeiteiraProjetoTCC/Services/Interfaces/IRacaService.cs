using GestaoLeiteiraProjetoTCC.Models;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IRacaService
    {
        Task Criar(string nomeRaca);
        Task<bool> ValidarSeExisteRaca(string nomeRaca);
        Task<List<Raca>> ObterRacasOrdenadasPorNome();
        Task<Raca> ObterRacaPorId(int id);
        public Task ExcluirRaca(int id);
        public Task AtualizarRacaPorId(int id, string novoNome);
    }
}