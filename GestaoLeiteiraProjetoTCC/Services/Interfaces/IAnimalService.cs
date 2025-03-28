﻿using GestaoLeiteiraProjetoTCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface IAnimalService
    {
        Task<List<Animal>> ObterAnimaisDaPropriedadeAsync(int propriedadeId);
        Task<Animal> CadastrarAnimalAsync(Animal animal);
        Task<Animal> AtualizarAnimalAsync(Animal animal);
        Task<bool> ExcluirAnimalAsync(int id);
    }
}
