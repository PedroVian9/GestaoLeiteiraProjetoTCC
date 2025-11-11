using GestaoLeiteiraProjetoTCC.Data;
using GestaoLeiteiraProjetoTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;
    private bool _initialized;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DatabaseService()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_initialized)
        {
            return _database;
        }

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
            Console.WriteLine($"Erro na inicializacao do banco de dados: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        return _database;
    }

    private async Task InitializeAsync()
    {
        await _database.CreateTableAsync<Propriedade>();
        await _database.CreateTableAsync<Animal>();
        await _database.CreateTableAsync<Lactacao>();
        await _database.CreateTableAsync<ProducaoLeiteira>();
        await _database.CreateTableAsync<Raca>();
        await _database.CreateTableAsync<QuantidadeOrdenha>();
        await _database.CreateTableAsync<Gestacao>();

        await EnsureIndexesAsync();
        await EnsureSyncInfrastructureAsync();

        Console.WriteLine($"Banco de dados pronto em: {Constants.DatabasePath}");

        await SeedRacasAsync();
        await SeedQuantidadeOrdenhaAsync();
    }

    private async Task EnsureIndexesAsync()
    {
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PropriedadeId ON Animal (PropriedadeId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_MaeId ON Animal (MaeId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_PaiId ON Animal (PaiId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Lactacao_AnimalId ON Lactacao (AnimalId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_ProducaoLeiteira_AnimalId ON ProducaoLeiteira (AnimalId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_ProducaoLeiteira_LactacaoId ON ProducaoLeiteira (LactacaoId)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Animal_RacaId ON Animal (RacaId)");

        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_Animal_SyncId ON Animal (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_Propriedade_SyncId ON Propriedade (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_Raca_SyncId ON Raca (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_Lactacao_SyncId ON Lactacao (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_ProducaoLeiteira_SyncId ON ProducaoLeiteira (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_Gestacao_SyncId ON Gestacao (SyncId)");
        await _database.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS UX_QuantidadeOrdenha_SyncId ON QuantidadeOrdenha (SyncId)");
    }

    private async Task EnsureSyncInfrastructureAsync()
    {
        var tables = new[]
        {
            "Propriedade",
            "Animal",
            "Lactacao",
            "ProducaoLeiteira",
            "Raca",
            "QuantidadeOrdenha",
            "Gestacao"
        };

        foreach (var table in tables)
        {
            await EnsureColumnAsync(table, "SyncId", "TEXT NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'");
            await EnsureColumnAsync(table, "UpdatedAt", "TEXT NOT NULL DEFAULT '1970-01-01T00:00:00Z'");
            await EnsureColumnAsync(table, "IsDeleted", "INTEGER NOT NULL DEFAULT 0");
            await EnsureColumnAsync(table, "LastChangedByDevice", "TEXT NOT NULL DEFAULT ''");
        }

        await BackfillSyncMetadataAsync<Propriedade>();
        await BackfillSyncMetadataAsync<Animal>();
        await BackfillSyncMetadataAsync<Lactacao>();
        await BackfillSyncMetadataAsync<ProducaoLeiteira>();
        await BackfillSyncMetadataAsync<Raca>();
        await BackfillSyncMetadataAsync<QuantidadeOrdenha>();
        await BackfillSyncMetadataAsync<Gestacao>();
    }

    private async Task EnsureColumnAsync(string tableName, string columnName, string definition)
    {
        var columnExists = await _database.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM pragma_table_info('{tableName}') WHERE name = '{columnName}'");

        if (columnExists == 0)
        {
            await _database.ExecuteAsync($"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition}");
        }
    }

    private async Task BackfillSyncMetadataAsync<T>() where T : ISyncEntity, new()
    {
        var records = await _database.Table<T>().ToListAsync();
        foreach (var record in records)
        {
            var needsUpdate = false;
            if (record.SyncId == Guid.Empty)
            {
                record.SyncId = Guid.NewGuid();
                needsUpdate = true;
            }

            if (record.UpdatedAt == DateTime.MinValue || record.UpdatedAt.Year < 2000)
            {
                record.UpdatedAt = DateTime.UtcNow;
                needsUpdate = true;
            }

            if (record.LastChangedByDevice == null)
            {
                record.LastChangedByDevice = string.Empty;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                await _database.UpdateAsync(record);
            }
        }
    }

    private async Task SeedRacasAsync()
    {
        var racasExistentes = await _database.Table<Raca>().CountAsync();
        if (racasExistentes > 0)
        {
            return;
        }

        var racasMock = new List<Raca>
        {
            new Raca { NomeRaca = "Holandesa", Status = "Sistema" },
            new Raca { NomeRaca = "Jersey", Status = "Sistema" },
            new Raca { NomeRaca = "Girolando", Status = "Sistema" },
            new Raca { NomeRaca = "Gir Leiteiro", Status = "Sistema" },
            new Raca { NomeRaca = "Guzera Leiteiro", Status = "Sistema" },
        };
        await _database.InsertAllAsync(racasMock);
    }

    private async Task SeedQuantidadeOrdenhaAsync()
    {
        var quantidadeOrdenhaExistente = await _database.Table<QuantidadeOrdenha>().CountAsync();
        if (quantidadeOrdenhaExistente > 0)
        {
            return;
        }

        var ordenhaPadrao = new QuantidadeOrdenha
        {
            Quantidade = 2,
            DataRegistro = DateTime.UtcNow
        };

        await _database.InsertAsync(ordenhaPadrao);
    }
}
