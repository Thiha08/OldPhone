using System.Collections.Concurrent;

namespace OldPhone.UI.Web.Services
{
    /// <summary>
    /// Thread-safe implementation of session management for phone application
    /// </summary>
    public class PhoneSessionManager : IPhoneSessionManager
    {
        private readonly ILogger<PhoneSessionManager> _logger;
        private readonly ConcurrentDictionary<string, ClientInfo> _activeConnections;
        private readonly object _lockObject = new object();

        public PhoneSessionManager(ILogger<PhoneSessionManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activeConnections = new ConcurrentDictionary<string, ClientInfo>();
        }

        public async Task RegisterClientAsync(string connectionId, string? userName = null)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            try
            {
                var clientInfo = new ClientInfo
                {
                    ConnectionId = connectionId,
                    UserName = userName ?? "Anonymous",
                    ConnectedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };

                if (_activeConnections.TryAdd(connectionId, clientInfo))
                {
                    _logger.LogInformation("Client registered: {ConnectionId}, User: {UserName}, Total connections: {Count}", 
                        connectionId, userName ?? "Anonymous", _activeConnections.Count);
                }
                else
                {
                    _logger.LogWarning("Failed to register client: {ConnectionId} - already exists", connectionId);
                }

                await Task.CompletedTask; // Async operation for future extensibility
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering client: {ConnectionId}", connectionId);
                throw;
            }
        }

        public async Task UnregisterClientAsync(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            try
            {
                if (_activeConnections.TryRemove(connectionId, out var clientInfo))
                {
                    var duration = DateTime.UtcNow - clientInfo.ConnectedAt;
                    _logger.LogInformation("Client unregistered: {ConnectionId}, User: {UserName}, Duration: {Duration}, Total connections: {Count}", 
                        connectionId, clientInfo.UserName, duration, _activeConnections.Count);
                }
                else
                {
                    _logger.LogWarning("Failed to unregister client: {ConnectionId} - not found", connectionId);
                }

                await Task.CompletedTask; // Async operation for future extensibility
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering client: {ConnectionId}", connectionId);
                throw;
            }
        }

        public int GetActiveConnectionCount()
        {
            return _activeConnections.Count;
        }

        public IEnumerable<string> GetActiveConnections()
        {
            return _activeConnections.Keys.ToList();
        }

        public bool IsConnectionActive(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                return false;
            }

            return _activeConnections.ContainsKey(connectionId);
        }

        /// <summary>
        /// Updates the last activity time for a connection
        /// </summary>
        /// <param name="connectionId">The connection ID</param>
        public void UpdateActivity(string connectionId)
        {
            if (_activeConnections.TryGetValue(connectionId, out var clientInfo))
            {
                clientInfo.LastActivity = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets connection statistics
        /// </summary>
        /// <returns>Connection statistics</returns>
        public ConnectionStatistics GetStatistics()
        {
            var now = DateTime.UtcNow;
            var activeConnections = _activeConnections.Values.ToList();

            return new ConnectionStatistics
            {
                TotalConnections = activeConnections.Count,
                AverageConnectionDuration = activeConnections.Any() 
                    ? TimeSpan.FromTicks((long)activeConnections.Average(c => (now - c.ConnectedAt).Ticks))
                    : TimeSpan.Zero,
                OldestConnection = activeConnections.Any() 
                    ? activeConnections.Min(c => c.ConnectedAt)
                    : DateTime.UtcNow,
                NewestConnection = activeConnections.Any() 
                    ? activeConnections.Max(c => c.ConnectedAt)
                    : DateTime.UtcNow
            };
        }

        /// <summary>
        /// Cleans up stale connections (for future use with connection timeouts)
        /// </summary>
        /// <param name="timeout">Connection timeout</param>
        public void CleanupStaleConnections(TimeSpan timeout)
        {
            var cutoffTime = DateTime.UtcNow - timeout;
            var staleConnections = _activeConnections
                .Where(kvp => kvp.Value.LastActivity < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var connectionId in staleConnections)
            {
                if (_activeConnections.TryRemove(connectionId, out var clientInfo))
                {
                    _logger.LogInformation("Removed stale connection: {ConnectionId}, Last activity: {LastActivity}", 
                        connectionId, clientInfo.LastActivity);
                }
            }
        }

        /// <summary>
        /// Represents client connection information
        /// </summary>
        private class ClientInfo
        {
            public string ConnectionId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public DateTime ConnectedAt { get; set; }
            public DateTime LastActivity { get; set; }
        }

        /// <summary>
        /// Represents connection statistics
        /// </summary>
        public class ConnectionStatistics
        {
            public int TotalConnections { get; set; }
            public TimeSpan AverageConnectionDuration { get; set; }
            public DateTime OldestConnection { get; set; }
            public DateTime NewestConnection { get; set; }
        }
    }
} 