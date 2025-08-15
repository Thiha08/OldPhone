namespace OldPhone.UI.Shared.Models.SignalR
{
    /// <summary>
    /// Session join data
    /// </summary>
    public record SessionJoinData : BaseSignalRModel
    {
        public string ConnectionId { get; init; } = string.Empty;
    }

    /// <summary>
    /// Session leave data
    /// </summary>
    public record SessionLeaveData : BaseSignalRModel
    {
        public string ConnectionId { get; init; } = string.Empty;
    }
} 