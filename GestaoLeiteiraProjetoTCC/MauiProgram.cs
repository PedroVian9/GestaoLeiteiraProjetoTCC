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
        builder.Services.AddSingleton<IPropriedadeRepository, PropriedadeRepository>();
        builder.Services.AddSingleton<IPropriedadeService, PropriedadeService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Inicialização segura do banco de dados
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