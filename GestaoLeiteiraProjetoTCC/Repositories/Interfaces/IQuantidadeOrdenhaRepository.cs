using GestaoLeiteiraProjetoTCC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories.Interfaces
{
    public interface IQuantidadeOrdenhaRepository
    {
        Task<QuantidadeOrdenha> CadastrarAsync(QuantidadeOrdenha quantidadeOrdenha);
        Task<QuantidadeOrdenha> ObterMaisRecenteAsync();
    }
}