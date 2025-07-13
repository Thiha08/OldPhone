using OldPhone.Core.Processor.Services;

namespace OldPhone.Core.Processor.Tests
{
    public class OldPhoneKeyServiceTests
    {
        [Theory]
        [InlineData("222 2 22#", "CAB")]
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

        [Theory]
        [InlineData("222 2 22#", "CAB")]
        [InlineData("33#", "E")]
        [InlineData("227*#", "B")]
        [InlineData("4433555 555666#", "HELLO")]
        [InlineData("8 88777444666*664#", "TURING")]
        public void TextCompleted_Event_FiresExpectedOutput(string input, string expectedOutput)
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            string? eventText = null;
            keyService.TextCompleted += text => eventText = text;

            // Act
            keyService.OldPhonePad(input);

            // Assert
            Assert.Equal(expectedOutput, eventText);
        }

        [Fact]
        public void TextChanged_Event_FiresExpectedOutput()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            var textChanges = new List<string>();
            keyService.TextChanged += text => textChanges.Add(text);

            // Act
            keyService.OldPhonePad("33#");

            // Assert
            Assert.Equal(2, textChanges.Count); // "D", "E"
            Assert.Equal("D", textChanges[0]);
            Assert.Equal("E", textChanges[1]);
        }

        [Theory]
        [InlineData('2', "A", "B", "C")]
        [InlineData('3', "D", "E", "F")]
        [InlineData('4', "G", "H", "I")]
        [InlineData('5', "J", "K", "L")]
        [InlineData('6', "M", "N", "O")]
        [InlineData('7', "P", "Q", "R", "S")]
        [InlineData('8', "T", "U", "V")]
        [InlineData('9', "W", "X", "Y", "Z")]

        public void ProcessKey_ExpectedOutput(char key, params string[] expectedChars)
        {
            // Arrange
            var keyService = new OldPhoneKeyService();

            // Act
            for (int i = 0; i < expectedChars.Length; i++)
            {
                keyService.ProcessKey(key);

                // Assert
                Assert.Equal(expectedChars[i], keyService.CurrentText);
            }
        }

        [Fact]
        public void ProcessKey_InvalidKeyOutput()
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
        public void ProcessTimeout_ExpectedOutput()
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

        [Fact]
        public void ProcessCleaning_ExpectedOutput()
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
        public void ProcessCompleted_ReturnExpectedOutput()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();
            string? eventText = null;
            keyService.TextCompleted += text => eventText = text;

            // Act
            keyService.ProcessKey('2'); // A
            keyService.ProcessKey('2'); // B (cycling)
            keyService.ProcessComplete();

            // Assert
            Assert.Equal("B", eventText);
            Assert.Equal("", keyService.CurrentText);
        }

        [Fact]
        public void Dispose_DisposesResources_ExpectedOutput()
        {
            // Arrange
            var keyService = new OldPhoneKeyService();

            // Act & Assert - Should not throw
            keyService.Dispose();

            // Should handle multiple dispose calls
            keyService.Dispose();
        }
    }
}
