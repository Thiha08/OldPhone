using OldPhone.ConsoleApp.Services;

namespace OldPhone.Tests
{
    public class OldPhonePadTests
    {
        [Theory]
        [InlineData("222 2 22", "CAB")]
        [InlineData("33#", "E")]
        [InlineData("227*#", "B")]
        [InlineData("4433555 555666#", "HELLO")]
        [InlineData("8 88777444666*664#", "TURING")]
        public void OldPhonePad_ReturnsExpectedOutput(string input, string expectedOutput)
        {
            // Arrange
            var keyService = new OldPhoneKeyService();

            // Act
            var result = keyService.OldPhonePad(input);

            // Assert
            Assert.Equal(expectedOutput, result);
        }

        [Fact]
        public void ProcessCleaning_ClearsAllText()
        {
            // Arrange - Add 'A'
            var keyService = new OldPhoneKeyService();
            keyService.ProcessKey('2');

            // Act
            keyService.ProcessCleaning();

            // Assert
            Assert.Equal("", keyService.CurrentText);
        }

        [Fact]
        public void ProcessKey_InvalidKey_DoesNothing()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            string initialText = keyService.CurrentText;

            // Act - Invalid key
            keyService.ProcessKey('X');

            // Assert
            Assert.Equal(initialText, keyService.CurrentText);
        }

        [Fact]
        public void Dispose_DisposesResources()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();

            // Act & Assert - Should not throw
            keyService.Dispose();
            keyService.Dispose(); // Should handle multiple dispose calls
        }

        [Fact]
        public void OnTimedEvent_ResetsCurrentKey()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            keyService.ProcessKey('2'); // 'A'
            keyService.ProcessKey('2'); // 'B'

            // Act
            keyService.ProcessTimeout();

            // Assert - Next '2' should append 'A' (not continue cycling)
            keyService.ProcessKey('2');
            Assert.Equal("BA", keyService.CurrentText);
        }
    }
}
