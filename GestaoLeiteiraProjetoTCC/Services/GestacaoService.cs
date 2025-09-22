using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class GestacaoService : IGestacaoService
    {
        private readonly IGestacaoRepository _gestacaoRepository;
        private readonly IAnimalRepository _animalRepository;

        public GestacaoService(IGestacaoRepository gestacaoRepository, IAnimalRepository animalRepository)
        {
            _gestacaoRepository = gestacaoRepository;
            _animalRepository = animalRepository;
        }

        public async Task<List<Gestacao>> ObterCiclosAtivos(int propriedadeId)
        {
            var animaisDaPropriedade = await _animalRepository.ObterAnimaisPorPropriedadeIdDb(propriedadeId);

            var idsAnimais = animaisDaPropriedade.Select(a => a.Id).ToList();

            return await _gestacaoRepository.ObterCiclosAtivosPorPropriedadeDb(idsAnimais);
        }

        public async Task<Gestacao> IniciarCicloAsync(Gestacao ciclo)
        {
            ciclo.Status = "Em Cobertura";
            return await _gestacaoRepository.IniciarGestacaoDb(ciclo);
        }

        public async Task<Gestacao> ConfirmarGestacaoAsync(int cicloId, DateTime dataConfirmacao)
        {
            var ciclo = await _gestacaoRepository.ObterGestacaoPorIdDb(cicloId);
            if (ciclo != null)
            {
                ciclo.Status = "Gestação Ativa";
                ciclo.DataConfirmacao = dataConfirmacao;
                return await _gestacaoRepository.AtualizarGestacaoDb(ciclo);
            }
            return null;
        }

        public async Task FinalizarGestacaoComCriaVivaAsync(int cicloId, Animal cria)
        {
            var criaCadastrada = await _animalRepository.CadastrarAnimalDb(cria);
            var ciclo = await _gestacaoRepository.ObterGestacaoPorIdDb(cicloId);

            if (ciclo != null)
            {
                ciclo.Status = "Finalizada - Parto";
                ciclo.DataFim = cria.DataNascimento ?? DateTime.Today;
                ciclo.CriaId = criaCadastrada.Id;
                await _gestacaoRepository.AtualizarGestacaoDb(ciclo);

                var mae = await _animalRepository.ObterAnimalPorIdDb(ciclo.VacaId);
                if (mae != null)
                {
                    mae.Lactante = true;
                    mae.NumeroDePartos += 1;
                    mae.DataUltimoParto = ciclo.DataFim;
                    await _animalRepository.AtualizarAnimalDb(mae);
                }
            }
        }

        public async Task FinalizarGestacaoSemCriaVivaAsync(int cicloId, string statusFinal)
        {
            var ciclo = await _gestacaoRepository.ObterGestacaoPorIdDb(cicloId);
            if (ciclo != null)
            {
                ciclo.Status = statusFinal;
                ciclo.DataFim = DateTime.Today;
                await _gestacaoRepository.AtualizarGestacaoDb(ciclo);

                var mae = await _animalRepository.ObterAnimalPorIdDb(ciclo.VacaId);
                if (mae != null)
                {
                    mae.DataUltimoParto = ciclo.DataFim;
                    if (statusFinal.Contains("Nascimorto"))
                    {
                        mae.NumeroDePartos += 1;
                        mae.NumeroDeNascimortos += 1;
                    }
                    else if (statusFinal.Contains("Aborto"))
                    {
                        mae.NumeroDeAbortos += 1;
                    }
                    await _animalRepository.AtualizarAnimalDb(mae);
                }
            }
        }
    }
}