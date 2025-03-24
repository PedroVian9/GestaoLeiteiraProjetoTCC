using SQLite;
using GestaoLeiteiraProjetoTCC.Data;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public SQLiteAsyncConnection GetConnection() => _database;

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<Models.Propriedade>();
        }
    }
}
