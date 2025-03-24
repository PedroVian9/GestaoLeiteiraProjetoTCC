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
    }
}