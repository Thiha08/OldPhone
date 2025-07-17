namespace OldPhone.Core.Shared.Entities
{
    /// <summary>
    /// User message in the old phone system
    /// </summary>
    public class UserMessage : BaseEntity<string>
    {
        public string SessionId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}