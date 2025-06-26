using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OldPhone.ConsoleApp.Services;

namespace OldPhone.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var app = host.Services.GetRequiredService<OldPhoneApp>();
            app.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IOldPhoneKeyService, OldPhoneKeyService>();
                    services.AddTransient<OldPhoneApp>();
                });
    }

   
}
