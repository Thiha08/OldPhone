using FluentValidation;
using Microsoft.Extensions.Logging;
using OldPhone.Core.Exceptions;
using OldPhone.Core.Shared.DTOs;
using OldPhone.Core.Shared.Entities;
using OldPhone.Infrastructure.Repositories;

namespace OldPhone.Core.Services
{
    public class UserMessageService : IUserMessageService
    {
        private readonly IRepository<UserMessage, string> _repository;
        private readonly ILogger<UserMessageService> _logger;
        private readonly IValidator<UserMessage> _validator;

        public UserMessageService(
            IRepository<UserMessage, string> repository,
            ILogger<UserMessageService> logger,
            IValidator<UserMessage> validator)
        {
            _repository = repository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<IEnumerable<UserMessageDto>> GetAllAsync()
        {
            try
            {
                var messages = await _repository.GetAllAsync();
                var dtos = messages.Select(MapToUserMessageDto).ToList();
                _logger.LogDebug("Retrieved {Count} user messages", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user messages");
                throw;
            }
        }

        public async Task<UserMessageDto> CreateAsync(string sessionId, string content)
        {
            try
            {
                var message = new UserMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = sessionId,
                    Content = content
                };

                var validationResult = await _validator.ValidateAsync(message);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Message validation failed: {Errors}", 
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new EntityValidationException("Message validation failed", validationResult);
                }

                var createdMessage = await _repository.AddAsync(message);
                
                return MapToUserMessageDto(createdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user message for session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<UserMessageDto>> GetBySessionIdAsync(string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("GetBySessionIdAsync called with null or empty session ID");
                    return Enumerable.Empty<UserMessageDto>();
                }

                var allMessages = await _repository.GetAllAsync();
                var sessionMessages = allMessages.Where(m => m.SessionId == sessionId).ToList();
                var dtos = sessionMessages.Select(MapToUserMessageDto).ToList();
                
                _logger.LogDebug("Retrieved {Count} messages for session: {SessionId}", 
                    dtos.Count, sessionId);
                
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages for session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<MessageStatisticsDto> GetMessageStatisticsAsync()
        {
            try
            {
                var allMessages = await _repository.GetAllAsync();
                
                // Calculate statistics
                var totalMessages = allMessages.Count();
                
                var messagesPerSession = allMessages
                    .GroupBy(m => m.SessionId)
                    .ToDictionary(g => g.Key, g => g.Count());

                var activeSessions = messagesPerSession.Count;

                var statistics = new MessageStatisticsDto
                {
                    TotalMessages = totalMessages,
                    ActiveSessions = activeSessions,
                    MessagesPerSession = messagesPerSession
                };

                _logger.LogDebug("Retrieved message statistics: Total={Total}, Sessions={Sessions}, Details={Details}", 
                    totalMessages, activeSessions, 
                    string.Join(", ", messagesPerSession.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
                
                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving message statistics");
                throw;
            }
        }

        /// <summary>
        /// Maps a UserMessage entity to UserMessageDto
        /// </summary>
        /// <param name="message">The entity to map</param>
        /// <returns>The mapped DTO</returns>
        private static UserMessageDto MapToUserMessageDto(UserMessage message)
        {
            return new UserMessageDto
            {
                Id = message.Id,
                SessionId = message.SessionId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
} 