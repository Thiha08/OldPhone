using FluentValidation;
using OldPhone.Core.Shared.Entities;

namespace OldPhone.Core.Validators
{
    /// <summary>
    /// FluentValidation validator for UserMessage entity
    /// </summary>
    public class UserMessageValidator : AbstractValidator<UserMessage>
    {
        public UserMessageValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Message ID cannot be empty")
                .MaximumLength(50)
                .WithMessage("Message ID cannot exceed 50 characters");

            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("Session ID cannot be empty")
                .MaximumLength(100)
                .WithMessage("Session ID cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithMessage("Session ID can only contain alphanumeric characters, hyphens, and underscores");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Message content cannot be empty")
                .MaximumLength(1000)
                .WithMessage("Message content cannot exceed 1000 characters")
                .MinimumLength(1)
                .WithMessage("Message content must be at least 1 character long");

            // Content should not contain suspicious patterns
            RuleFor(x => x.Content)
                .Must(content => !ContainsSuspiciousPatterns(content))
                .WithMessage("Message content contains suspicious patterns");
        }

        

        /// <summary>
        /// Checks for suspicious patterns in content
        /// </summary>
        private static bool ContainsSuspiciousPatterns(string content)
        {
            var suspiciousPatterns = new[]
            {
                "script",
                "javascript:",
                "onload=",
                "onerror=",
                "eval(",
                "document.cookie",
                "<script",
                "</script>"
            };

            var lowerContent = content.ToLowerInvariant();
            return suspiciousPatterns.Any(pattern => lowerContent.Contains(pattern));
        }
    }
} 