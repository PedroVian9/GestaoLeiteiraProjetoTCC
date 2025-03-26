using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using SQLite;
using System;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class ProducaoLeiteiraService : IProducaoLeiteiraService
    {
        private readonly IProducaoLeiteiraRepository _producaoLeiteiraRepository;
        private readonly SQLiteAsyncConnection _database;

        public ProducaoLeiteiraService(IProducaoLeiteiraRepository producaoLeiteiraRepository, SQLiteAsyncConnection database)
        {
            _producaoLeiteiraRepository = producaoLeiteiraRepository;
            _database = database;
        }

        public async Task<int> CriarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira)
        {
            return await _producaoLeiteiraRepository.CriarProducaoLeiteiraDb(producaoLeiteira);
        }

        public async Task AtualizarProducaoLeiteiraAsync(ProducaoLeiteira producaoLeiteira)
        {
            await _producaoLeiteiraRepository.AtualizarProducaoLeiteiraDb(producaoLeiteira);
        }
    }
}