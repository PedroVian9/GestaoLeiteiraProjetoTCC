using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class PropriedadeService : IPropriedadeService
    {
        private readonly IPropriedadeRepository _propriedadeRepository;
        private Propriedade _propriedadeLogada;

        public PropriedadeService(IPropriedadeRepository propriedadeRepository)
        {
            _propriedadeRepository = propriedadeRepository;
        }

        public async Task<Propriedade> CadastrarPropriedadeAsync(Propriedade propriedade)
        {
            return await _propriedadeRepository.CadastrarPropriedadeDb(propriedade);
        }

        public async Task<List<Propriedade>> ObterTodasPropriedadesAsync()
        {
            return await _propriedadeRepository.ObterTodasPropriedadesDb();
        }

        public async Task<Propriedade> ObterPropriedadePorIdAsync(int id)
        {
            return await _propriedadeRepository.ObterPropriedadePorIdDb(id);
        }

        public async Task<Propriedade> LoginPropriedadeAsync(string nomeProprietario, string senha)
        {
            _propriedadeLogada = await _propriedadeRepository.ValidarLoginDb(nomeProprietario, senha);
            return _propriedadeLogada;
        }

        public Propriedade ObterPropriedadeLogada()
        {
            return _propriedadeLogada;
        }

        public async Task<List<Animal>> ObterAnimaisDaPropriedadeLogadaAsync()
        {
            if (_propriedadeLogada == null)
            {
                throw new InvalidOperationException("Nenhuma propriedade está logada.");
            }

            return await _propriedadeRepository.ObterAnimaisPorPropriedadeIdDb(_propriedadeLogada.Id);
        }
    }
}