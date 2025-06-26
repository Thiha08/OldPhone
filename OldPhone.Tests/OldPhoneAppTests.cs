using OldPhone.ConsoleApp;
using OldPhone.ConsoleApp.Services;
using Xunit;

namespace OldPhone.Tests
{
    public class OldPhoneAppTests
    {
        [Theory]
        [InlineData("33#", "E")]
        [InlineData("227*#", "B")]
        [InlineData("4433555 555666#", "HELLO")]
        [InlineData("8 88777444666*664#", "TURING")]
        public void OldPhonePad_ReturnsExpectedOutput(string input, string expectedOutput)
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            var app = new OldPhoneApp(keyService);

            // Act
            var result = keyService.Process(input);

            // Assert
            Assert.Equal(expectedOutput, result);
        }
    }
}
