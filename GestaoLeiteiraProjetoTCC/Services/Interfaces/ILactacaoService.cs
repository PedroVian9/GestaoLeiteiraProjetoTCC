namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ILactacaoService
    {
        Task<int> CriarLactacaoAsync(Lactacao lactacao);

        Task<List<Lactacao>> ObterLactacoesPorAnimalAsync(int animalId);

        Task FinalizarLactacaoAsync(int id);

        bool PodeEditarLactacao(int id);
    }
}