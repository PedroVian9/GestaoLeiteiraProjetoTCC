using GestaoLeiteiraProjetoTCC.Data;
using GestaoLeiteiraProjetoTCC.Models;
using SQLite;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                if (!_initialized)
                {
                    await InitializeAsync();
                    _initialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicialização do banco de dados: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
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
            // Criar tabelas
            await _database.CreateTableAsync<Propriedade>();
            await _database.CreateTableAsync<Animal>();

            // Criar índices para simular foreign keys
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PropriedadeId ON Animal (PropriedadeId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_MaeId ON Animal (MaeId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PaiId ON Animal (PaiId)");

            Console.WriteLine($"Banco de dados criado em: {Constants.DatabasePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao criar banco de dados: {ex.Message}");
            Console.WriteLine($"Detalhes do erro: {ex.GetType().FullName}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

}