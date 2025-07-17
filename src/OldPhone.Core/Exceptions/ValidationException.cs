using FluentValidation.Results;

namespace OldPhone.Core.Exceptions
{
    /// <summary>
    /// Custom exception for entity validation errors
    /// </summary>
    public class EntityValidationException : Exception
    {
        public ValidationResult ValidationResult { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidationException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="validationResult">The validation result from FluentValidation</param>
        public EntityValidationException(string message, ValidationResult validationResult) 
            : base(message)
        {
            ValidationResult = validationResult;
        }

        /// <summary>
        /// Gets a formatted error message with all validation errors
        /// </summary>
        /// <returns>Formatted error message</returns>
        public string GetFormattedErrorMessage()
        {
            if (!ValidationResult.Errors.Any())
                return Message;

            var errorDetails = ValidationResult.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();

            return $"{Message}\nValidation Errors:\n{string.Join("\n", errorDetails)}";
        }
    }
} 