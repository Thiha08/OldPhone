namespace OldPhone.UI.Shared.Models.SignalR
{
    /// <summary>
    /// Base model for SignalR data models containing common properties
    /// </summary>
    public abstract record BaseSignalRModel
    {
        /// <summary>
        /// Session identifier for grouping related operations
        /// </summary>
        public string SessionId { get; init; } = string.Empty;

        /// <summary>
        /// Timestamp when the event occurred
        /// </summary>
        public DateTime Timestamp { get; init; }
    }

    /// <summary>
    /// Base model for SignalR data models that only need timestamp
    /// </summary>
    public abstract record BaseTimestampModel
    {
        /// <summary>
        /// Timestamp when the event occurred
        /// </summary>
        public DateTime Timestamp { get; init; }
    }
} 