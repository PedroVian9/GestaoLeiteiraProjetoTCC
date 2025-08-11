using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IPropriedadeService
    {
        Task<Propriedade> CadastrarPropriedadeAsync(Propriedade propriedade);
        Task<List<Propriedade>> ObterTodasPropriedadesAsync();
        Task<Propriedade> ObterPropriedadePorIdAsync(int id);
        Task<Propriedade> LoginPropriedadeAsync(string nomeProprietario, string senha);
        Propriedade ObterPropriedadeLogada();
        Task<List<Animal>> ObterAnimaisDaPropriedadeLogadaAsync();
        Task<bool> EditarPropriedadeAsync(Propriedade propriedade);
        Task<bool> AlterarSenhaAsync(int id, string novaSenha);
    }
}