using GestaoLeiteiraProjetoTCC;
using GestaoLeiteiraProjetoTCC.Repositories;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // Registrar serviços
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<ISyncMetadataService, SyncMetadataService>();
        builder.Services.AddSingleton<ISyncTransportService, SyncTransportService>();

        builder.Services.AddSingleton<IPropriedadeRepository, PropriedadeRepository>();
        builder.Services.AddSingleton<IAnimalRepository, AnimalRepository>();
        builder.Services.AddSingleton<ILactacaoRepository, LactacaoRepository>();
        builder.Services.AddSingleton<IProducaoLeiteiraRepository, ProducaoLeiteiraRepository>();
        builder.Services.AddSingleton<IRacaRepository, RacaRepository>();
        builder.Services.AddSingleton<IQuantidadeOrdenhaRepository, QuantidadeOrdenhaRepository>();
        builder.Services.AddSingleton<IGestacaoRepository, GestacaoRepository>();

        builder.Services.AddSingleton<IPropriedadeService, PropriedadeService>();
        builder.Services.AddSingleton<IAnimalService, AnimalService>();
        builder.Services.AddSingleton<ILactacaoService, LactacaoService>();
        builder.Services.AddSingleton<IProducaoLeiteiraService, ProducaoLeiteiraService>();
        builder.Services.AddSingleton<IRacaService, RacaService>();
        builder.Services.AddSingleton<IQuantidadeOrdenhaService, QuantidadeOrdenhaService>();
        builder.Services.AddSingleton<IGestacaoService, GestacaoService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<ISyncAutomationService, SyncAutomationService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        InitializeDatabase(app.Services);

        return app;
    }

    private static void InitializeDatabase(IServiceProvider services)
    {
        try
        {
            var dbService = services.GetRequiredService<DatabaseService>();
            Task.Run(async () =>
            {
                var connection = await dbService.GetConnectionAsync();
                Console.WriteLine("Banco de dados inicializado com sucesso!");
            }).Wait(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha crítica na inicialização do banco: {ex}");
            throw;
        }
    }

}
