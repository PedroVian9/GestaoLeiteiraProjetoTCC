using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class QuantidadeOrdenhaService : IQuantidadeOrdenhaService
    {
        private readonly IQuantidadeOrdenhaRepository _quantidadeOrdenhaRepository;
        public QuantidadeOrdenhaService(IQuantidadeOrdenhaRepository quantidadeOrdenhaRepository)
        {
            _quantidadeOrdenhaRepository = quantidadeOrdenhaRepository;
        }

        public async Task<QuantidadeOrdenha> CadastrarQuantidadeOrdenha(int quantidade)
        {
            if (quantidade < 1 || quantidade > 3)
            {
                throw new ArgumentException("A quantidade de ordenhas deve ser 1, 2 ou 3.", nameof(quantidade));
            }

            var novaOrdenha = new QuantidadeOrdenha
            {
                Quantidade = quantidade
            };

            return await _quantidadeOrdenhaRepository.CadastrarAsync(novaOrdenha);
        }

        public async Task<QuantidadeOrdenha> ObterQuantidadeOrdenhaMaisRecente()
        {
            return await _quantidadeOrdenhaRepository.ObterMaisRecenteAsync();
        }
    }
}