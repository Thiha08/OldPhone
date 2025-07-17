namespace OldPhone.Core.Shared.DTOs
{
    public class CreateUserMessageDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }

    public class UserMessageDto
    {
        public string Id { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class MessageStatisticsDto
    {
        public int TotalMessages { get; set; }

        public int ActiveSessions { get; set; }

        public Dictionary<string, int> MessagesPerSession { get; set; } = new();
    }
}
