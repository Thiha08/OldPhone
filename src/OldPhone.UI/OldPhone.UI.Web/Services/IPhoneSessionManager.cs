namespace OldPhone.UI.Web.Services
{
    /// <summary>
    /// Manages client sessions and connections for the phone application
    /// </summary>
    public interface IPhoneSessionManager
    {
        /// <summary>
        /// Registers a new client connection
        /// </summary>
        /// <param name="connectionId">The SignalR connection ID</param>
        /// <param name="userName">Optional user name</param>
        /// <returns>Task representing the async operation</returns>
        Task RegisterClientAsync(string connectionId, string? userName = null);

        /// <summary>
        /// Unregisters a client connection
        /// </summary>
        /// <param name="connectionId">The SignalR connection ID</param>
        /// <returns>Task representing the async operation</returns>
        Task UnregisterClientAsync(string connectionId);

        /// <summary>
        /// Gets the current number of active connections
        /// </summary>
        /// <returns>The number of active connections</returns>
        int GetActiveConnectionCount();

        /// <summary>
        /// Gets all active connection IDs
        /// </summary>
        /// <returns>Collection of active connection IDs</returns>
        IEnumerable<string> GetActiveConnections();

        /// <summary>
        /// Checks if a connection is active
        /// </summary>
        /// <param name="connectionId">The connection ID to check</param>
        /// <returns>True if the connection is active, false otherwise</returns>
        bool IsConnectionActive(string connectionId);
    }
} 