using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OldPhone.UI.MultiApp.Services;
using OldPhone.UI.Shared.Services;
using OldPhone.UI.Shared.Models.SignalR;
using OldPhone.UI.Shared.Services.SignalR;
using System.Reflection;

namespace OldPhone.UI.MultiApp
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

            // Configure app settings
            var assembly = Assembly.GetExecutingAssembly();
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.appsettings.json")!)
                .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.appsettings.Development.json")!)
                .Build();

            // Add device-specific services used by the OldPhone.UI.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddSingleton<IConfiguration>(configuration);

            builder.Services.AddSignalRClient(builder.Configuration);

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
