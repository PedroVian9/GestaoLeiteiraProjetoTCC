namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ILactacaoService
    {
        Task<int> CriarLactacaoAsync(Lactacao lactacao);

        Task<List<Lactacao>> ObterLactacoesPorAnimalAsync(int animalId);

        Task FinalizarLactacaoAsync(int id);
        Task<List<Lactacao>> ObterTodasLactacoesAtivasAsync(int propriedadeId);

        bool PodeEditarLactacao(int id);
        Task<List<Lactacao>> ObterLactacoesDaPropriedadeAsync(int propriedadeId);
    }
}