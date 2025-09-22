using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class GestacaoRepository : IGestacaoRepository
    {
        private readonly DatabaseService _databaseService;

        public GestacaoRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // This is the new, corrected method implementation
        public async Task<List<Gestacao>> ObterCiclosAtivosPorPropriedadeDb(List<int> idsAnimaisDaPropriedade)
        {
            var db = await _databaseService.GetConnectionAsync();
            var activeStatuses = new List<string> { "Em Cobertura", "Gestação Ativa" };

            return await db.Table<Gestacao>()
                           .Where(g => activeStatuses.Contains(g.Status) && idsAnimaisDaPropriedade.Contains(g.VacaId))
                           .ToListAsync();
        }

        public async Task<Gestacao> IniciarGestacaoDb(Gestacao gestacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.InsertAsync(gestacao);
            return gestacao;
        }

        public async Task<Gestacao> AtualizarGestacaoDb(Gestacao gestacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.UpdateAsync(gestacao);
            return gestacao;
        }

        public async Task<Gestacao> ObterGestacaoPorIdDb(int gestacaoId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Gestacao>().FirstOrDefaultAsync(g => g.Id == gestacaoId);
        }
    }
}