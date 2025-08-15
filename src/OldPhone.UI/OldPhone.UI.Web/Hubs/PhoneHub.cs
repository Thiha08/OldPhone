using Microsoft.AspNetCore.SignalR;
using OldPhone.Core.Services;
using OldPhone.UI.Shared.Models.SignalR;
using OldPhone.UI.Web.Services;
using ConnectionInfo = OldPhone.UI.Shared.Models.SignalR.ConnectionInfo;

namespace OldPhone.UI.Web.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time phone keypad communication
    /// Handles key presses, text processing, and client synchronization
    /// </summary>
    public class PhoneHub : Hub<IPhoneHub>
    {
        private readonly ILogger<PhoneHub> _logger;
        private readonly IPhoneServiceFactory _phoneServiceFactory;
        private readonly IPhoneSessionManager _sessionManager;
        private readonly HashSet<string> _registeredSessions = new HashSet<string>();

        public PhoneHub(
            ILogger<PhoneHub> logger,
            IPhoneServiceFactory phoneServiceFactory,
            IPhoneSessionManager sessionManager)
        {
            _logger = logger;
            _phoneServiceFactory = phoneServiceFactory;
            _sessionManager = sessionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userAgent = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            
            _logger.LogInformation("Client connected: {ConnectionId}, User-Agent: {UserAgent}", 
                connectionId, userAgent);

            await _sessionManager.RegisterClientAsync(connectionId, Context.User?.Identity?.Name);
            
            // Register phone service events for the default session
            RegisterPhoneServiceEvents("default");
            
            var connectionInfo = new ConnectionInfo
            {
                ConnectionId = connectionId,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.Caller.Connected(connectionInfo);
            
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Registers event handlers for a phone service session
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        private void RegisterPhoneServiceEvents(string sessionId)
        {
            try
            {
                // Check if events are already registered for this session
                if (_registeredSessions.Contains(sessionId))
                {
                    _logger.LogDebug("Phone service events already registered for session {SessionId}", sessionId);
                    return;
                }

                var phoneService = _phoneServiceFactory.GetPhoneService(sessionId);
                
                // Subscribe to TextCompleted event for this session
                phoneService.TextCompleted += async (completedText) =>
                {
                    try
                    {
                        _logger.LogInformation("TextCompleted event fired for session {SessionId}: {Text}", 
                            sessionId, completedText);

                        //// Save the completed text to Redis
                        //if (!string.IsNullOrEmpty(completedText))
                        //{
                        //    var savedMessage = await _userMessageService.CreateAsync(sessionId, completedText);
                        //    _logger.LogInformation("Message saved to Redis: {MessageId} for session {SessionId}", 
                        //        savedMessage.Id, sessionId);
                        //}

                        // Notify all clients about text completion
                        var targetClients = string.IsNullOrEmpty(sessionId) 
                            ? Clients.All 
                            : Clients.Group(sessionId);

                        var textCompletedData = new TextCompletedData
                        {
                            CompletedBy = "system", // System event, not a specific client
                            SessionId = sessionId,
                            Timestamp = DateTime.UtcNow
                        };

                        await targetClients.TextCompleted(textCompletedData);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling TextCompleted event for session {SessionId}", sessionId);
                    }
                };

                // Mark this session as registered
                _registeredSessions.Add(sessionId);
                _logger.LogDebug("Phone service events registered for session {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering phone service events for session {SessionId}", sessionId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected: {ConnectionId}", connectionId);
            }

            await _sessionManager.UnregisterClientAsync(connectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Processes a key press from a client
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        /// <param name="sessionId">Optional session identifier for multi-session support</param>
        public async Task ProcessKeyPress(string key, string? sessionId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogWarning("Empty key received from {ConnectionId}", Context.ConnectionId);
                    await Clients.Caller.Error("Invalid key: key cannot be empty");
                    return;
                }

                if (key.Length != 1)
                {
                    _logger.LogWarning("Invalid key length received from {ConnectionId}: {Key}", 
                        Context.ConnectionId, key);
                    await Clients.Caller.Error("Invalid key: must be a single character");
                    return;
                }

                var keyChar = key[0];
                var connectionId = Context.ConnectionId;

                _logger.LogDebug("Processing key '{Key}' from {ConnectionId} in session {SessionId}", 
                    keyChar, connectionId, sessionId ?? "default");

                // Get the appropriate phone service for the session
                var phoneService = _phoneServiceFactory.GetPhoneService(sessionId ?? "default");

                // Process the key using the phone service
                phoneService.ProcessKey(keyChar);

                // Get the current text after processing
                var currentText = phoneService.CurrentText;

                // Broadcast to all clients in the same session
                var targetClients = string.IsNullOrEmpty(sessionId) 
                    ? Clients.All 
                    : Clients.Group(sessionId);

                var keyProcessedData = new KeyProcessedData
                {
                    Key = key,
                    CurrentText = currentText,
                    ProcessedBy = connectionId,
                    SessionId = sessionId ?? "default",
                    Timestamp = DateTime.UtcNow
                };

                await targetClients.KeyProcessed(keyProcessedData);

                _logger.LogDebug("Key '{Key}' processed successfully for {ConnectionId} in session {SessionId}, current text: {Text}", 
                    keyChar, connectionId, sessionId ?? "default", currentText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing key '{Key}' from {ConnectionId}", key, Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while processing the key");
            }
        }

        /// <summary>
        /// Processes text completion (send button)
        /// </summary>
        /// <param name="sessionId">Optional session identifier</param>
        public async Task ProcessTextComplete(string? sessionId = null)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                _logger.LogInformation("Text completion requested by {ConnectionId} in session {SessionId}", 
                    connectionId, sessionId ?? "default");

                // Get the appropriate phone service for the session
                var phoneService = _phoneServiceFactory.GetPhoneService(sessionId ?? "default");

                phoneService.ProcessComplete();

                var targetClients = string.IsNullOrEmpty(sessionId) 
                    ? Clients.All 
                    : Clients.Group(sessionId);

                var textCompletedData = new TextCompletedData
                {
                    CompletedBy = connectionId,
                    SessionId = sessionId ?? "default",
                    Timestamp = DateTime.UtcNow
                };

                await targetClients.TextCompleted(textCompletedData);

                _logger.LogInformation("Text completed successfully by {ConnectionId} in session {SessionId}", 
                    connectionId, sessionId ?? "default");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing text for {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while completing text");
            }
        }

        /// <summary>
        /// Clears the current text
        /// </summary>
        /// <param name="sessionId">Optional session identifier</param>
        public async Task ClearText(string? sessionId = null)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                _logger.LogDebug("Text clear requested by {ConnectionId} in session {SessionId}", 
                    connectionId, sessionId ?? "default");

                // Get the appropriate phone service for the session
                var phoneService = _phoneServiceFactory.GetPhoneService(sessionId ?? "default");

                phoneService.ProcessCleaning();

                var targetClients = string.IsNullOrEmpty(sessionId) 
                    ? Clients.All 
                    : Clients.Group(sessionId);

                var textClearedData = new TextClearedData
                {
                    ClearedBy = connectionId,
                    SessionId = sessionId ?? "default",
                    Timestamp = DateTime.UtcNow
                };

                await targetClients.TextCleared(textClearedData);

                _logger.LogDebug("Text cleared successfully by {ConnectionId} in session {SessionId}", 
                    connectionId, sessionId ?? "default");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing text for {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while clearing text");
            }
        }

        /// <summary>
        /// Joins a specific session/room
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        public async Task JoinSession(string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    await Clients.Caller.Error("Session ID cannot be empty");
                    return;
                }

                var connectionId = Context.ConnectionId;
                await Groups.AddToGroupAsync(connectionId, sessionId);
                
                // Register phone service events for this session
                RegisterPhoneServiceEvents(sessionId);
                
                _logger.LogInformation("Client {ConnectionId} joined session {SessionId}", connectionId, sessionId);
                
                var sessionJoinData = new SessionJoinData
                {
                    ConnectionId = connectionId,
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow
                };

                await Clients.Group(sessionId).ClientJoinedSession(sessionJoinData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining session {SessionId} for {ConnectionId}", 
                    sessionId, Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while joining the session");
            }
        }

        /// <summary>
        /// Leaves a specific session/room
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        public async Task LeaveSession(string sessionId)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                await Groups.RemoveFromGroupAsync(connectionId, sessionId);
                
                _logger.LogInformation("Client {ConnectionId} left session {SessionId}", connectionId, sessionId);
                
                var sessionLeaveData = new SessionLeaveData
                {
                    ConnectionId = connectionId,
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow
                };

                await Clients.Group(sessionId).ClientLeftSession(sessionLeaveData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving session {SessionId} for {ConnectionId}", 
                    sessionId, Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while leaving the session");
            }
        }

        /// <summary>
        /// Gets the current state of the phone service
        /// </summary>
        /// <param name="sessionId">Optional session identifier</param>
        public async Task GetCurrentState(string? sessionId = null)
        {
            try
            {
                // Get the appropriate phone service for the session
                var phoneService = _phoneServiceFactory.GetPhoneService(sessionId ?? "default");

                var currentStateData = new CurrentStateData
                {
                    CurrentText = phoneService.CurrentText,
                    SessionId = sessionId ?? "default",
                    Timestamp = DateTime.UtcNow
                };

                await Clients.Caller.CurrentState(currentStateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current state for {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while getting current state");
            }
        }

        /// <summary>
        /// Gets server statistics
        /// </summary>
        public async Task GetServerStatistics()
        {
            try
            {
                var serverStatisticsData = new ServerStatisticsData
                {
                    ActiveConnections = _sessionManager.GetActiveConnectionCount(),
                    ActiveSessions = _phoneServiceFactory.GetActiveSessionCount(),
                    Timestamp = DateTime.UtcNow
                };

                await Clients.Caller.ServerStatistics(serverStatisticsData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server statistics for {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.Error("An error occurred while getting server statistics");
            }
        }
    }
} 