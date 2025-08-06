using GestaoLeiteiraProjetoTCC.Models;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IRacaRepository
    {
        Task<bool> ExisteRacaAsync(string nomeRaca);
        Task CriarAsync(Raca raca);
        Task<List<Raca>> ObterTodasOrdenadasPorNomeAsync();
        Task<Raca> ObterPorIdAsync(int id);
        Task AtualizarAsync(Raca raca);
        Task ExcluirAsync(int id);
    }
}