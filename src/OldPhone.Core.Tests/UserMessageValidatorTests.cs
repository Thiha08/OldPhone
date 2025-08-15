using FluentValidation.TestHelper;
using OldPhone.Core.Shared.Entities;
using OldPhone.Core.Validators;
using Xunit;

namespace OldPhone.Core.Tests
{
    public class UserMessageValidatorTests
    {
        private readonly UserMessageValidator _validator;

        public UserMessageValidatorTests()
        {
            _validator = new UserMessageValidator();
        }

        [Fact]
        public void Should_Pass_When_Valid_Message()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id-123",
                SessionId = "session-123",
                Content = "Hello, this is a valid message!",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Id_Is_Empty()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "",
                SessionId = "session-123",
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Fail_When_SessionId_Is_Empty()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "",
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Fail_When_SessionId_Contains_Invalid_Characters()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session@123", // @ is not allowed
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Fail_When_Content_Is_Empty()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_Fail_When_Content_Is_Too_Long()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = new string('a', 1001), // 1001 characters, exceeds 1000 limit
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_Fail_When_Content_Is_Only_Whitespace()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "   \t\n   ",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_Fail_When_Content_Is_Too_Repetitive()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "aaaaaaa", // Too many repeated characters
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_Fail_When_Content_Contains_Suspicious_Patterns()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "Hello <script>alert('xss')</script>",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_Fail_When_CreatedAt_Is_In_Future()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow.AddDays(1), // Future date
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CreatedAt);
        }

        [Fact]
        public void Should_Fail_When_UpdatedAt_Is_Before_CreatedAt()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow.AddDays(-1) // Before CreatedAt
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UpdatedAt);
        }

        [Fact]
        public void Should_Pass_When_UpdatedAt_Is_Null()
        {
            // Arrange
            var message = new UserMessage
            {
                Id = "test-id",
                SessionId = "session-123",
                Content = "Valid content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Act
            var result = _validator.TestValidate(message);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UpdatedAt);
        }
    }
} 