# SignalR Configuration Setup

This document explains how configuration is set up for both MAUI and Blazor WASM clients to work with the shared `ISignalRConfigurationService`.

## Configuration Files

### MAUI App Configuration

The MAUI app uses embedded JSON configuration files:

- **`src/OldPhone.UI/OldPhone.UI/appsettings.json`** - Production settings
- **`src/OldPhone.UI/OldPhone.UI/appsettings.Development.json`** - Development settings

These files are embedded as resources in the MAUI project and loaded at runtime.

### Blazor WASM Configuration

The Blazor WASM client uses standard `wwwroot` configuration files:

- **`src/OldPhone.UI/OldPhone.UI.Web.Client/wwwroot/appsettings.json`** - Production settings
- **`src/OldPhone.UI/OldPhone.UI.Web.Client/wwwroot/appsettings.Development.json`** - Development settings

These files are served as static assets and loaded by the Blazor WASM runtime.

## Configuration Structure

Both platforms use the same configuration structure:

```json
{
  "SignalR": {
    "ServerUrl": "https://localhost:7001/phonehub",
    "UseSecureConnection": true,
    "ConnectionTimeoutSeconds": 30,
    "ReconnectionDelaySeconds": 5,
    "MaxReconnectionAttempts": 10
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Configuration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SignalR:ServerUrl` | string | `"https://localhost:7001/phonehub"` | SignalR hub URL |
| `SignalR:UseSecureConnection` | bool | `true` | Whether to use HTTPS |
| `SignalR:ConnectionTimeoutSeconds` | int | `30` | Connection timeout in seconds |
| `SignalR:ReconnectionDelaySeconds` | int | `5` | Delay between reconnection attempts |
| `SignalR:MaxReconnectionAttempts` | int | `10` | Maximum reconnection attempts |

## Platform-Specific Settings

### MAUI Settings
- **Production**: Longer timeouts (30s connection, 5s reconnection delay)
- **Development**: Shorter timeouts (15s connection, 3s reconnection delay)

### Blazor WASM Settings
- **Production**: Medium timeouts (20s connection, 3s reconnection delay)
- **Development**: Short timeouts (10s connection, 2s reconnection delay)

## Implementation Details

### Shared Service
The `SignalRConfigurationService` in `OldPhone.UI.Shared` reads configuration from `IConfiguration`:

```csharp
public class SignalRConfigurationService : ISignalRConfigurationService
{
    private readonly IConfiguration _configuration;

    public SignalRConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string SignalRServerUrl => 
        _configuration["SignalR:ServerUrl"] ?? "https://localhost:7001/phonehub";
    
    // ... other properties
}
```

### MAUI Configuration Loading
In `MauiProgram.cs`, configuration is loaded from embedded resources:

```csharp
var assembly = Assembly.GetExecutingAssembly();
var configuration = new ConfigurationBuilder()
    .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.MultiApp.appsettings.json")!)
    .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.MultiApp.appsettings.Development.json")!)
    .Build();

builder.Services.AddSingleton<IConfiguration>(configuration);
```

### Blazor WASM Configuration Loading
Blazor WASM automatically loads configuration from `wwwroot/appsettings.json` files when using `WebAssemblyHostBuilder.CreateDefault(args)`.

## Usage

Both platforms register the shared service:

```csharp
builder.Services.AddSingleton<ISignalRConfigurationService, SignalRConfigurationService>();
```

The service can then be injected and used in any component or service that needs SignalR configuration.

## Environment-Specific Configuration

- **Development**: Uses `appsettings.Development.json` with shorter timeouts and debug logging
- **Production**: Uses `appsettings.json` with longer timeouts and info logging

## Security Considerations

- All configurations default to secure connections (`UseSecureConnection: true`)
- Server URLs should use HTTPS in production
- Configuration files should not contain sensitive information (use environment variables for secrets)

## Deployment Notes

### MAUI
- Configuration files are embedded in the app bundle
- No external configuration files needed at runtime
- Settings can be changed by updating the embedded resources

### Blazor WASM
- Configuration files are served as static assets
- Can be updated without rebuilding the app
- Files are publicly accessible (don't include secrets) 