namespace OldPhone.UI.Shared.Models.SignalR
{
    /// <summary>
    /// Connection information sent when a client connects
    /// </summary>
    public record ConnectionInfo : BaseTimestampModel
    {
        public string ConnectionId { get; init; } = string.Empty;
    }
} 