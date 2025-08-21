using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class GestacaoService : IGestacaoService
    {
        private readonly IGestacaoRepository _gestacaoRepository;
        private readonly IAnimalRepository _animalRepository;

        // O DatabaseService foi removido daqui. O serviço não precisa conhecê-lo.
        public GestacaoService(IGestacaoRepository gestacaoRepository, IAnimalRepository animalRepository)
        {
            _gestacaoRepository = gestacaoRepository;
            _animalRepository = animalRepository;
        }

        public async Task<List<Gestacao>> ObterGestoesAtivas(int propriedadeId)
        {
            var animaisDaPropriedade = await _animalRepository.ObterAnimaisPorPropriedadeIdDb(propriedadeId);
            var idsAnimais = animaisDaPropriedade.Select(a => a.Id).ToList();
            return await _gestacaoRepository.ObterGestoesAtivasPorPropriedadeDb(idsAnimais);
        }

        public async Task<Gestacao> IniciarGestacaoAsync(Gestacao gestacao)
        {
            return await _gestacaoRepository.IniciarGestacaoDb(gestacao);
        }

        public async Task<Gestacao> AtualizarGestacaoAsync(Gestacao gestacao)
        {
            return await _gestacaoRepository.AtualizarGestacaoDb(gestacao);
        }

        // MÉTODO COMPLETAMENTE REFATORADO
        public async Task FinalizarGestacaoAsync(int gestacaoId, Animal cria)
        {
            // 1. Cadastra o novo animal (a cria) usando o repositório de animais
            var criaCadastrada = await _animalRepository.CadastrarAnimalDb(cria);

            // 2. Busca a gestação usando o repositório de gestações
            var gestacao = await _gestacaoRepository.ObterGestacaoPorIdDb(gestacaoId);

            if (gestacao != null)
            {
                // 3. Atualiza os dados da gestação
                gestacao.Status = "Finalizada - Parto";
                gestacao.DataFim = DateTime.Today;
                gestacao.CriaId = criaCadastrada.Id; // Vincula a cria à gestação
                await _gestacaoRepository.AtualizarGestacaoDb(gestacao);

                // 4. Atualiza o status da mãe para "Lactante"
                var mae = await _animalRepository.ObterAnimalPorIdDb(gestacao.VacaId);
                if (mae != null)
                {
                    mae.Lactante = true;
                    await _animalRepository.AtualizarAnimalDb(mae);
                }
            }
        }
    }
}