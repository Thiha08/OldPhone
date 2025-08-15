namespace OldPhone.UI.Shared.Models.SignalR
{
    /// <summary>
    /// Current state data
    /// </summary>
    public record CurrentStateData : BaseSignalRModel
    {
        public string CurrentText { get; init; } = string.Empty;
    }

    /// <summary>
    /// Server statistics data
    /// </summary>
    public record ServerStatisticsData : BaseTimestampModel
    {
        public int ActiveConnections { get; init; }

        public int ActiveSessions { get; init; }
    }
} 