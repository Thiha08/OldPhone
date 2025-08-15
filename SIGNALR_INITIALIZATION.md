# SignalR Initialization Guide

This document explains how SignalR is properly initialized and connected to the hub for both MAUI and Blazor WASM clients.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    SignalR Backend                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   PhoneHub      │  │Session Manager  │  │Service      │ │
│  │  (SignalR Hub)  │  │(Connection Mgmt)│  │Factory      │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
│           │                    │                    │       │
│           └────────────────────┼────────────────────┘       │
│                                │                            │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              OldPhone.Core.Processor                   │ │
│  │           (Business Logic Services)                    │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                    SignalR Clients                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   MAUI App      │  │  Blazor WASM    │  │   Web       │ │
│  │  (Mobile/Desktop)│  │   (Browser)     │  │  (Server)   │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Backend Initialization (OldPhone.UI.Web)

### 1. SignalR Hub Registration
```csharp
// Program.cs
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 1024; // 1KB max message size
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
```

### 2. Hub Mapping
```csharp
// Program.cs
app.MapHub<PhoneHub>("/phonehub");
```

### 3. Service Registration
```csharp
// Program.cs
builder.Services.AddSingleton<IPhoneSessionManager, PhoneSessionManager>();
builder.Services.AddSingleton<IPhoneServiceFactory, PhoneServiceFactory>();
```

## Client Initialization

### MAUI App Initialization

#### 1. Configuration Loading
```csharp
// MauiProgram.cs
var assembly = Assembly.GetExecutingAssembly();
var configuration = new ConfigurationBuilder()
    .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.MultiApp.appsettings.json")!)
    .AddJsonStream(assembly.GetManifestResourceStream("OldPhone.UI.MultiApp.appsettings.Development.json")!)
    .Build();

builder.Services.AddSingleton<IConfiguration>(configuration);
```

#### 2. Service Registration
```csharp
// MauiProgram.cs
builder.Services.AddSingleton<ISignalRConfigurationService, SignalRConfigurationService>();
builder.Services.AddSingleton<ISignalRPhoneService, SignalRPhoneService>();
```

#### 3. Connection Initialization
```csharp
// App.xaml.cs
protected override async void OnStart()
{
    try
    {
        var services = this.Handler.MauiContext.Services;
        var signalRService = services.GetRequiredService<ISignalRPhoneService>();
        var configService = services.GetRequiredService<ISignalRConfigurationService>();

        // Connect using configuration service
        await signalRService.ConnectAsync(configService.SignalRServerUrl, configService.DefaultSessionId);
    }
    catch (Exception ex)
    {
        // Log error but don't crash the app
        System.Diagnostics.Debug.WriteLine($"Failed to connect to SignalR: {ex.Message}");
    }
}
```

### Blazor WASM Initialization

#### 1. Configuration Loading
Blazor WASM automatically loads configuration from `wwwroot/appsettings.json` files when using `WebAssemblyHostBuilder.CreateDefault(args)`.

#### 2. Service Registration
```csharp
// Program.cs
builder.Services.AddSingleton<ISignalRConfigurationService, SignalRConfigurationService>();
builder.Services.AddSingleton<ISignalRPhoneService, SignalRPhoneService>();
```

#### 3. Connection Initialization
```csharp
// Program.cs
var app = builder.Build();

// Initialize SignalR connection
try
{
    var signalRService = app.Services.GetRequiredService<ISignalRPhoneService>();
    var configService = app.Services.GetRequiredService<ISignalRConfigurationService>();
    
    // Connect using configuration service
    await signalRService.ConnectAsync(configService.SignalRServerUrl, configService.DefaultSessionId);
}
catch (Exception ex)
{
    // Log error but don't crash the app
    Console.WriteLine($"Failed to connect to SignalR: {ex.Message}");
}

await app.RunAsync();
```

## Configuration Files

### MAUI Configuration
- **Production**: `src/OldPhone.UI/OldPhone.UI/appsettings.json`
- **Development**: `src/OldPhone.UI/OldPhone.UI/appsettings.Development.json`

### Blazor WASM Configuration
- **Production**: `src/OldPhone.UI/OldPhone.UI.Web.Client/wwwroot/appsettings.json`
- **Development**: `src/OldPhone.UI/OldPhone.UI.Web.Client/wwwroot/appsettings.Development.json`

## Connection Process

### 1. Hub Connection Setup
```csharp
_hubConnection = new HubConnectionBuilder()
    .WithUrl($"{serverUrl.TrimEnd('/')}/phonehub")
    .WithAutomaticReconnect(new CustomRetryPolicy(_logger, _configService))
    .Build();
```

### 2. Event Handler Registration
```csharp
private void RegisterEventHandlers()
{
    // Connection events
    _hubConnection.Closed += (error) => { /* Handle disconnection */ };
    _hubConnection.Reconnecting += (error) => { /* Handle reconnection */ };
    _hubConnection.Reconnected += (connectionId) => { /* Handle reconnected */ };

    // Phone events
    _hubConnection.On<JsonElement>("KeyProcessed", (data) => { /* Handle key processed */ });
    _hubConnection.On<JsonElement>("TextCompleted", (data) => { /* Handle text completed */ });
    _hubConnection.On<JsonElement>("TextCleared", (data) => { /* Handle text cleared */ });
    _hubConnection.On<JsonElement>("CurrentState", (data) => { /* Handle current state */ });
    _hubConnection.On<string>("Error", (errorMessage) => { /* Handle errors */ });
}
```

### 3. Connection Start
```csharp
await _hubConnection.StartAsync();
```

## Retry Policy

The `CustomRetryPolicy` provides exponential backoff with jitter:

```csharp
public class CustomRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        if (retryContext.PreviousRetryCount >= GetMaxRetryAttempts())
        {
            return null; // Stop retrying
        }

        var delay = CalculateDelay(retryContext.PreviousRetryCount);
        return delay;
    }

    private TimeSpan CalculateDelay(int previousRetryCount)
    {
        // Exponential backoff with jitter
        var baseDelay = GetReconnectionDelaySeconds();
        var exponentialDelay = baseDelay * Math.Pow(2, previousRetryCount);
        
        // Add jitter to prevent thundering herd
        var jitter = Random.Shared.NextDouble() * 0.1; // 10% jitter
        var finalDelay = exponentialDelay * (1 + jitter);
        
        // Cap the maximum delay at 60 seconds
        var cappedDelay = Math.Min(finalDelay, 60);
        
        return TimeSpan.FromSeconds(cappedDelay);
    }
}
```

## Error Handling

### Connection Errors
- Automatic retry with exponential backoff
- Graceful degradation if connection fails
- User-friendly error messages

### Service Errors
- Comprehensive logging
- Event-based error notification
- Non-blocking error handling

## Testing the Connection

### 1. Start the Backend
```bash
cd src/OldPhone.UI/OldPhone.UI.Web
dotnet run
```

### 2. Test MAUI App
```bash
cd src/OldPhone.UI/OldPhone.UI
dotnet build
dotnet run
```

### 3. Test Blazor WASM
```bash
cd src/OldPhone.UI/OldPhone.UI.Web.Client
dotnet run
```

### 4. Verify Connection
- Check connection status indicator
- Try pressing phone keys
- Monitor console logs for connection events

## Troubleshooting

### Common Issues

1. **Connection Failed**
   - Check if backend is running
   - Verify server URL in configuration
   - Check network connectivity

2. **Configuration Not Loading**
   - Verify configuration files exist
   - Check file paths and names
   - Ensure proper JSON syntax

3. **Service Not Registered**
   - Check service registration in Program.cs/MauiProgram.cs
   - Verify dependency injection setup
   - Check for missing packages

### Debug Steps

1. Enable detailed logging
2. Check browser developer tools (Blazor WASM)
3. Monitor console output
4. Verify SignalR hub endpoint is accessible

## Security Considerations

- All connections use HTTPS by default
- Input validation on all hub methods
- Session-based isolation
- Configurable message size limits
- Proper error handling without information leakage 