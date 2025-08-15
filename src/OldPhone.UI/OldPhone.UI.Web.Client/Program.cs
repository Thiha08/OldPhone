using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OldPhone.UI.Shared.Services;
using OldPhone.UI.Shared.Models.SignalR;
using OldPhone.UI.Shared.Services.SignalR;
using OldPhone.UI.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the OldPhone.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSignalRClient(builder.Configuration);

builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

await app.RunAsync();
