using OldPhone.Core.Processor.Services;
using OldPhone.UI.Shared.Services;
using OldPhone.UI.Shared.Services.SignalR;
using OldPhone.UI.Web.Components;
using OldPhone.UI.Web.Hubs;
using OldPhone.UI.Web.Services;

namespace OldPhone.UI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            // Add SignalR
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = builder.Environment.IsDevelopment();
                options.MaximumReceiveMessageSize = 1024; // 1KB max message size
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            });

            // Add device-specific services used by the OldPhone.UI.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Add configuration (Blazor WASM automatically loads appsettings.json)
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Add core phone services
            builder.Services.AddSingleton<IOldPhoneKeyService, OldPhoneKeyService>();

            builder.Services.AddSignalRClient(builder.Configuration);

            builder.Services.AddSingleton<IPhoneSessionManager, PhoneSessionManager>();
            builder.Services.AddSingleton<IPhoneServiceFactory, PhoneServiceFactory>();

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(
                    typeof(OldPhone.UI.Shared._Imports).Assembly,
                    typeof(OldPhone.UI.Web.Client._Imports).Assembly);

            // Add SignalR hub
            app.MapHub<PhoneHub>("/hubs/phone");

            app.Run();
        }
    }
}
