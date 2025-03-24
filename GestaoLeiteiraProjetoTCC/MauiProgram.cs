using GestaoLeiteiraProjetoTCC.Repositories;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestaoLeiteiraProjetoTCC
{
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

            // Registrar DatabaseService como Singleton
            builder.Services.AddSingleton<DatabaseService>();

            // Registrar PropriedadeService usando o DatabaseService
            builder.Services.AddSingleton<IPropriedadeRepository, PropriedadeRepository>();
            builder.Services.AddSingleton<IPropriedadeService, PropriedadeService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
