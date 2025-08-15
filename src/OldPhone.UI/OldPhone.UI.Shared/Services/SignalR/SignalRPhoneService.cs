using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OldPhone.UI.Shared.Models.SignalR;

namespace OldPhone.UI.Shared.Services.SignalR
{
    /// <summary>
    /// SignalR-based phone service implementation for Blazor WebAssembly applications
    /// Provides real-time communication with the web backend
    /// </summary>
    public class SignalRPhoneService : ISignalRPhoneService, IDisposable
    {
        private readonly ILogger<SignalRPhoneService> _logger;
        private readonly SignalROptions _options;
        private HubConnection? _hubConnection;
        private string? _currentSessionId;
        private string _currentText = string.Empty;
        private bool _isConnected = false;
        private bool _disposed = false;

        // Events
        public event Action<string>? TextChanged;
        public event Action<string>? TextCompleted;
        public event Action? TextCleared;
        public event Action<string>? ErrorOccurred;
        public event Action<bool>? ConnectionStatusChanged;

        // Properties
        public string CurrentText => _currentText;
        public bool IsConnected => _isConnected;
        public string? CurrentSessionId => _currentSessionId;

        public SignalRPhoneService(
            ILogger<SignalRPhoneService> logger,
            IOptions<SignalROptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task ConnectAsync(string? sessionId = null)
        {
            try
            {
                if (_hubConnection != null)
                {
                    await DisconnectAsync();
                }

                _logger.LogInformation("Connecting to SignalR hub at: {ServerUrl}", _options.ServerUrl);

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_options.ServerUrl}")
                    .WithAutomaticReconnect()
                    .Build();

                // Register event handlers
                RegisterEventHandlers();

                // Start connection
                await _hubConnection.StartAsync();

                _isConnected = true;
                _currentSessionId = sessionId;
                ConnectionStatusChanged?.Invoke(true);

                _logger.LogInformation("Successfully connected to SignalR hub");

                // Join session if specified
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await JoinSessionAsync(sessionId);
                }

                // Get current state
                await GetCurrentStateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to SignalR hub at {ServerUrl}", _options.ServerUrl);
                ErrorOccurred?.Invoke($"Failed to connect: {ex.Message}");
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }

                _isConnected = false;
                _currentSessionId = null;
                ConnectionStatusChanged?.Invoke(false);

                _logger.LogInformation("Disconnected from SignalR hub");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting from SignalR hub");
                ErrorOccurred?.Invoke($"Failed to disconnect: {ex.Message}");
            }
        }

        public async Task ProcessKeyAsync(string key)
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("Key cannot be null or empty", nameof(key));
                }

                _logger.LogDebug("Processing key: {Key}", key);

                await _hubConnection.InvokeAsync("ProcessKeyPress", key, _currentSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing key: {Key}", key);
                ErrorOccurred?.Invoke($"Failed to process key: {ex.Message}");
                throw;
            }
        }

        public async Task ForwardAsync()
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                _logger.LogDebug("Forward operation (Redis memory)");

                await _hubConnection.InvokeAsync("Forward", _currentSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forward operation");
                ErrorOccurred?.Invoke($"Failed to forward: {ex.Message}");
                throw;
            }
        }

        public async Task BackwardAsync()
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                _logger.LogDebug("Backward operation (Redis memory)");

                await _hubConnection.InvokeAsync("Backward", _currentSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in backward operation");
                ErrorOccurred?.Invoke($"Failed to backward: {ex.Message}");
                throw;
            }
        }

        public async Task GetCurrentStateAsync()
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                await _hubConnection.InvokeAsync("GetCurrentState", _currentSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current state");
                ErrorOccurred?.Invoke($"Failed to get current state: {ex.Message}");
                throw;
            }
        }

        public async Task JoinSessionAsync(string sessionId)
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
                }

                _logger.LogInformation("Joining session: {SessionId}", sessionId);

                await _hubConnection.InvokeAsync("JoinSession", sessionId);
                _currentSessionId = sessionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining session: {SessionId}", sessionId);
                ErrorOccurred?.Invoke($"Failed to join session: {ex.Message}");
                throw;
            }
        }

        public async Task LeaveSessionAsync()
        {
            try
            {
                if (_hubConnection?.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException("Not connected to SignalR hub");
                }

                if (!string.IsNullOrEmpty(_currentSessionId))
                {
                    _logger.LogInformation("Leaving session: {SessionId}", _currentSessionId);

                    await _hubConnection.InvokeAsync("LeaveSession", _currentSessionId);
                    _currentSessionId = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving session: {SessionId}", _currentSessionId);
                ErrorOccurred?.Invoke($"Failed to leave session: {ex.Message}");
                throw;
            }
        }

        private void RegisterEventHandlers()
        {
            if (_hubConnection == null) return;

            // Connection events
            _hubConnection.Closed += (error) =>
            {
                _isConnected = false;
                ConnectionStatusChanged?.Invoke(false);
                _logger.LogWarning("SignalR connection closed: {Error}", error?.Message ?? "No error");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnecting += (error) =>
            {
                _isConnected = false;
                ConnectionStatusChanged?.Invoke(false);
                _logger.LogInformation("SignalR reconnecting: {Error}", error?.Message ?? "No error");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _isConnected = true;
                ConnectionStatusChanged?.Invoke(true);
                _logger.LogInformation("SignalR reconnected: {ConnectionId}", connectionId);
                return Task.CompletedTask;
            };

            // Phone events - now using strongly typed models
            _hubConnection.On<KeyProcessedData>("KeyProcessed", (data) =>
            {
                try
                {
                    _currentText = data.CurrentText;
                    TextChanged?.Invoke(data.CurrentText);
                    _logger.LogDebug("Key processed, current text: {Text}, processed by: {ProcessedBy}", 
                        data.CurrentText, data.ProcessedBy);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling KeyProcessed event");
                }
            });

            _hubConnection.On<TextCompletedData>("TextCompleted", (data) =>
            {
                try
                {
                    var completedText = _currentText;
                    _currentText = string.Empty;
                    TextCompleted?.Invoke(completedText);
                    _logger.LogInformation("Text completed: {Text} by {CompletedBy}", 
                        completedText, data.CompletedBy);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling TextCompleted event");
                }
            });

            _hubConnection.On<TextClearedData>("TextCleared", (data) =>
            {
                try
                {
                    _currentText = string.Empty;
                    TextCleared?.Invoke();
                    _logger.LogDebug("Text cleared by {ClearedBy}", data.ClearedBy);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling TextCleared event");
                }
            });

            _hubConnection.On<CurrentStateData>("CurrentState", (data) =>
            {
                try
                {
                    _currentText = data.CurrentText;
                    TextChanged?.Invoke(data.CurrentText);
                    _logger.LogDebug("Current state received: {Text} for session {SessionId}", 
                        data.CurrentText, data.SessionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling CurrentState event");
                }
            });

            _hubConnection.On<string>("Error", (errorMessage) =>
            {
                _logger.LogWarning("Server error: {Error}", errorMessage);
                ErrorOccurred?.Invoke(errorMessage);
            });

            _hubConnection.On<ConnectionInfo>("Connected", (data) =>
            {
                _logger.LogInformation("Connected to SignalR hub with connection ID: {ConnectionId}", 
                    data.ConnectionId);
            });

            // Session management events
            _hubConnection.On<SessionJoinData>("ClientJoinedSession", (data) =>
            {
                _logger.LogInformation("Client {ConnectionId} joined session {SessionId}", 
                    data.ConnectionId, data.SessionId);
            });

            _hubConnection.On<SessionLeaveData>("ClientLeftSession", (data) =>
            {
                _logger.LogInformation("Client {ConnectionId} left session {SessionId}", 
                    data.ConnectionId, data.SessionId);
            });

            _hubConnection.On<ServerStatisticsData>("ServerStatistics", (data) =>
            {
                _logger.LogInformation("Server statistics - Active connections: {Connections}, Active sessions: {Sessions}", 
                    data.ActiveConnections, data.ActiveSessions);
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _hubConnection?.DisposeAsync().AsTask().Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing SignalR connection");
                }

                _disposed = true;
            }
        }
    }
} 