using OldPhone.Core.Shared.DTOs;

namespace OldPhone.Core.Services
{
    public interface IUserMessageService
    {
        /// <summary>
        /// Gets all user messages
        /// </summary>
        /// <returns>Collection of all user messages</returns>
        Task<IEnumerable<UserMessageDto>> GetAllAsync();

        /// <summary>
        /// Creates a new user message
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="content">The message content</param>
        /// <returns>The created user message</returns>
        Task<UserMessageDto> CreateAsync(string sessionId, string content);

        /// <summary>
        /// Gets all messages for a specific session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>Collection of messages for the session</returns>
        Task<IEnumerable<UserMessageDto>> GetBySessionIdAsync(string sessionId);

        /// <summary>
        /// Gets message statistics
        /// </summary>
        /// <returns>Message statistics</returns>
        Task<MessageStatisticsDto> GetMessageStatisticsAsync();
    }
} 