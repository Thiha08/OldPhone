namespace OldPhone.UI.Shared.Services.SignalR
{
    /// <summary>
    /// Interface for SignalR-based phone service for Blazor WebAssembly applications
    /// Provides real-time communication with the web backend
    /// </summary>
    public interface ISignalRPhoneService
    {
        /// <summary>
        /// Event fired when the current text changes
        /// </summary>
        event Action<string> TextChanged;

        /// <summary>
        /// Event fired when text is completed
        /// </summary>
        event Action<string> TextCompleted;

        /// <summary>
        /// Event fired when text is cleared
        /// </summary>
        event Action TextCleared;

        /// <summary>
        /// Event fired when an error occurs
        /// </summary>
        event Action<string> ErrorOccurred;

        /// <summary>
        /// Event fired when connection status changes
        /// </summary>
        event Action<bool> ConnectionStatusChanged;

        /// <summary>
        /// Gets the current text
        /// </summary>
        string CurrentText { get; }

        /// <summary>
        /// Gets the current connection status
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the current session ID
        /// </summary>
        string? CurrentSessionId { get; }

        /// <summary>
        /// Connects to the SignalR hub
        /// </summary>
        /// <param name="serverUrl">The server URL</param>
        /// <param name="sessionId">Optional session ID</param>
        /// <returns>Task representing the async operation</returns>
        Task ConnectAsync(string? sessionId = null);

        /// <summary>
        /// Disconnects from the SignalR hub
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Processes a key press
        /// </summary>
        /// <param name="key">The key to process</param>
        /// <returns>Task representing the async operation</returns>
        Task ProcessKeyAsync(string key);

        /// <summary>
        /// Completes the current text
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task ForwardAsync();

        /// <summary>
        /// Clears the current text
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task BackwardAsync();

        /// <summary>
        /// Gets the current state from the server
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task GetCurrentStateAsync();

        /// <summary>
        /// Joins a specific session
        /// </summary>
        /// <param name="sessionId">Session ID to join</param>
        /// <returns>Task representing the async operation</returns>
        Task JoinSessionAsync(string sessionId);

        /// <summary>
        /// Leaves the current session
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task LeaveSessionAsync();
    }
} 