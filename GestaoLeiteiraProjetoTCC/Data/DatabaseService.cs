using GestaoLeiteiraProjetoTCC.Data;
using SQLite;


public class DatabaseService
{
    private SQLiteAsyncConnection _database;
    private bool _initialized = false;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DatabaseService()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (!_initialized)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_initialized) // Double-check locking
                {
                    await InitializeAsync();
                    _initialized = true;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return _database;
    }

    private async Task InitializeAsync()
    {
        try
        {
            await _database.CreateTableAsync<GestaoLeiteiraProjetoTCC.Models.Propriedade>();
            // Adicione outras tabelas aqui conforme necessário
            Console.WriteLine($"Banco de dados criado em: {Constants.DatabasePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao criar banco de dados: {ex.Message}");
            throw;
        }
    }
}