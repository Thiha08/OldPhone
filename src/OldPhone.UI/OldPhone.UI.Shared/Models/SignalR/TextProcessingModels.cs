namespace OldPhone.UI.Shared.Models.SignalR
{
    /// <summary>
    /// Key processing result data
    /// </summary>
    public record KeyProcessedData : BaseSignalRModel
    {
        public string Key { get; init; } = string.Empty;

        public string CurrentText { get; init; } = string.Empty;

        public string ProcessedBy { get; init; } = string.Empty;
    }

    /// <summary>
    /// Text completion data
    /// </summary>
    public record TextCompletedData : BaseSignalRModel
    {
        public string CompletedBy { get; init; } = string.Empty;
    }

    /// <summary>
    /// Text clear data
    /// </summary>
    public record TextClearedData : BaseSignalRModel
    {
        public string ClearedBy { get; init; } = string.Empty;
    }
} 