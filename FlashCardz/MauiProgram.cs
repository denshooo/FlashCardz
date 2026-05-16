using Microsoft.Extensions.Logging;
using FlashCardz.Services;

namespace FlashCardz;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("PatrickHand-Regular.ttf", "Handwritten");
            });

        // ── Dependency Injection ──────────────────────────────
        // HttpClient registered as singleton so socket connections
        // are reused across the app (prevents socket exhaustion on Android).
        builder.Services.AddSingleton<HttpClient>();

        // Services
        builder.Services.AddSingleton<DeckService>();
        builder.Services.AddSingleton<AuthService>();

        // Pages — registered so DI can inject services into constructors
        builder.Services.AddTransient<Pages.LoginPage>();
        builder.Services.AddTransient<Pages.RegisterPage>();
        builder.Services.AddTransient<Pages.HomePage>();
        builder.Services.AddTransient<Pages.LearnPage>();
        builder.Services.AddTransient<Pages.EditDeckPage>();
        builder.Services.AddTransient<Pages.ProfilePage>();
        builder.Services.AddTransient<Pages.CreateDeckPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}