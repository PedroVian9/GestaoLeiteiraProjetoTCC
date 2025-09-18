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
            await _database.CreateTableAsync<Propriedade>();
            await _database.CreateTableAsync<Animal>();
            await _database.CreateTableAsync<Lactacao>();
            await _database.CreateTableAsync<ProducaoLeiteira>();
            await _database.CreateTableAsync<Raca>();
            await _database.CreateTableAsync<QuantidadeOrdenha>(); 
            await _database.CreateTableAsync<Gestacao>();

            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PropriedadeId ON Animal (PropriedadeId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_MaeId ON Animal (MaeId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PaiId ON Animal (PaiId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Lactacao_AnimalId ON Lactacao (AnimalId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_ProducaoLeiteira_AnimalId ON ProducaoLeiteira (AnimalId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_ProducaoLeiteira_LactacaoId ON ProducaoLeiteira (LactacaoId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_RacaId ON Animal (RacaId)");

            Console.WriteLine($"Banco de dados criado em: {Constants.DatabasePath}");

            var racasExistentes = await _database.Table<Raca>().CountAsync();
            if (racasExistentes == 0)
            {
                var racasMock = new List<Raca>
            {
                new Raca { NomeRaca = "Holandesa", Status = "Sistema" },
                new Raca { NomeRaca = "Jersey", Status = "Sistema" },
                new Raca { NomeRaca = "Girolando", Status = "Sistema" },
                new Raca { NomeRaca = "Gir Leiteiro", Status = "Sistema" },
                new Raca { NomeRaca = "Guzerá Leiteiro", Status = "Sistema" },
            };
                await _database.InsertAllAsync(racasMock);
            }

            // Verificar e criar a quantidade de ordenha padrão
            var quantidadeOrdenhaExistente = await _database.Table<QuantidadeOrdenha>().CountAsync();
            if (quantidadeOrdenhaExistente == 0)
            {
                var ordenhaPadrao = new QuantidadeOrdenha
                {
                    Quantidade = 2,
                    DataRegistro = DateTime.Now
                };
                await _database.InsertAsync(ordenhaPadrao);
            }
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