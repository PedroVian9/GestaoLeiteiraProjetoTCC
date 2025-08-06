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
    public class RacaService : IRacaService
    {
        private readonly IRacaRepository _racaRepository;

        public RacaService(IRacaRepository racaRepository)
        {
            _racaRepository = racaRepository;
        }

        public async Task Criar(string nomeRaca)
        {
            if (string.IsNullOrWhiteSpace(nomeRaca))
                throw new ArgumentException("O nome da raça é obrigatório.");

            bool existe = await _racaRepository.ExisteRacaAsync(nomeRaca);
            if (existe)
                throw new InvalidOperationException("Já existe uma raça com esse nome.");

            var novaRaca = new Raca { 
                NomeRaca = nomeRaca,
                Status = "Usuario"
            };
            await _racaRepository.CriarAsync(novaRaca);
        }

        public async Task<bool> ValidarSeExisteRaca(string nomeRaca)
        {
            return await _racaRepository.ExisteRacaAsync(nomeRaca);
        }

        public async Task<List<Raca>> ObterRacasOrdenadasPorNome()
        {
            return await _racaRepository.ObterTodasOrdenadasPorNomeAsync();
        }

        public async Task<Raca> ObterRacaPorId(int id)
        {
            return await _racaRepository.ObterPorIdAsync(id);
        }

        public async Task AtualizarRacaPorId(int id, string novoNome)
        {
            var raca = await _racaRepository.ObterPorIdAsync(id);

            if (raca == null)
                throw new Exception("Raça não encontrada.");

            if (raca.Status == "Sistema")
                throw new InvalidOperationException("Raças do sistema não podem ser editadas.");

            raca.NomeRaca = novoNome;
            await _racaRepository.AtualizarAsync(raca);
        }

        public async Task ExcluirRaca(int id)
        {
            var raca = await _racaRepository.ObterPorIdAsync(id);

            if (raca == null)
                throw new Exception("Raça não encontrada.");

            if (raca.Status == "Sistema")
                throw new InvalidOperationException("Raças do sistema não podem ser excluídas.");

            await _racaRepository.ExcluirAsync(id);
        }

    }
}