using GestaoLeiteiraProjetoTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class AnimalService
    {
        SQLiteAsyncConnection? database;

        public AnimalService(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Animal>().Wait();
        }

    }
}
