using FluentValidation;
using FluentValidation.Results;
using OldPhone.Core.Exceptions;
using Xunit;

namespace OldPhone.Core.Tests
{
    public class EntityValidationExceptionTests
    {
        [Fact]
        public void Constructor_WithValidationResult_ShouldStoreValidationResult()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Property1", "Error 1"),
                new ValidationFailure("Property2", "Error 2")
            };
            var validationResult = new ValidationResult(errors);
            var message = "Validation failed";

            // Act
            var exception = new EntityValidationException(message, validationResult);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(validationResult, exception.ValidationResult);
            Assert.Equal(2, exception.ValidationErrors.Count());
        }

        [Fact]
        public void Constructor_WithNullValidationResult_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new EntityValidationException("Test", (ValidationResult)null));
        }

        [Fact]
        public void GetErrorsForProperty_ShouldReturnErrorsForSpecificProperty()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Name", "Name is too short"),
                new ValidationFailure("Email", "Email is invalid")
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Test", validationResult);

            // Act
            var nameErrors = exception.GetErrorsForProperty("Name").ToList();
            var emailErrors = exception.GetErrorsForProperty("Email").ToList();

            // Assert
            Assert.Equal(2, nameErrors.Count);
            Assert.Contains("Name is required", nameErrors);
            Assert.Contains("Name is too short", nameErrors);
            Assert.Single(emailErrors);
            Assert.Contains("Email is invalid", emailErrors);
        }

        [Fact]
        public void GetErrorsByProperty_ShouldReturnGroupedErrors()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Name", "Name is too short"),
                new ValidationFailure("Email", "Email is invalid")
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Test", validationResult);

            // Act
            var errorsByProperty = exception.GetErrorsByProperty();

            // Assert
            Assert.Equal(2, errorsByProperty.Count);
            Assert.Equal(2, errorsByProperty["Name"].Count());
            Assert.Single(errorsByProperty["Email"]);
        }

        [Fact]
        public void GetFormattedErrorMessage_ShouldReturnFormattedMessage()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Email", "Email is invalid")
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Validation failed", validationResult);

            // Act
            var formattedMessage = exception.GetFormattedErrorMessage();

            // Assert
            Assert.Contains("Validation failed", formattedMessage);
            Assert.Contains("Name: Name is required", formattedMessage);
            Assert.Contains("Email: Email is invalid", formattedMessage);
        }

        [Fact]
        public void HasErrorsForProperty_ShouldReturnTrue_WhenPropertyHasErrors()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Test", validationResult);

            // Act & Assert
            Assert.True(exception.HasErrorsForProperty("Name"));
            Assert.False(exception.HasErrorsForProperty("Email"));
        }

        [Fact]
        public void GetErrorSeverities_ShouldReturnSeveritiesByProperty()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required") { Severity = Severity.Error },
                new ValidationFailure("Email", "Email is invalid") { Severity = Severity.Warning }
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Test", validationResult);

            // Act
            var severities = exception.GetErrorSeverities();

            // Assert
            Assert.Equal(2, severities.Count);
            Assert.Equal(Severity.Error, severities["Name"]);
            Assert.Equal(Severity.Warning, severities["Email"]);
        }

        [Fact]
        public void ValidationErrors_Property_ShouldReturnErrorMessages()
        {
            // Arrange
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Email", "Email is invalid")
            };
            var validationResult = new ValidationResult(errors);
            var exception = new EntityValidationException("Test", validationResult);

            // Act
            var errorMessages = exception.ValidationErrors.ToList();

            // Assert
            Assert.Equal(2, errorMessages.Count);
            Assert.Contains("Name is required", errorMessages);
            Assert.Contains("Email is invalid", errorMessages);
        }

        [Fact]
        public void Constructor_WithInnerException_ShouldStoreInnerException()
        {
            // Arrange
            var innerException = new InvalidOperationException("Inner error");
            var validationResult = new ValidationResult();
            var message = "Validation failed";

            // Act
            var exception = new EntityValidationException(message, innerException, validationResult);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(validationResult, exception.ValidationResult);
        }
    }
} 