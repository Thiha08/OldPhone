using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OldPhone.UI.Shared.Services;
using OldPhone.UI.Web.Client.Services;

namespace OldPhone.UI.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the OldPhone.UI.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            await builder.Build().RunAsync();
        }
    }
}
