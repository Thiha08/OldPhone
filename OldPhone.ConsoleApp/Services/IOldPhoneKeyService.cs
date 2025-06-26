namespace OldPhone.ConsoleApp.Services
{
    public interface IOldPhoneKeyService : IDisposable
    {
        string CurrentText { get; }

        event Action<string> TextChanged;

        /// <summary>
        /// Coding Challenge: Implement the OldPhonePad method to simulate the old phone keypad input.
        /// </summary>
        string OldPhonePad(string input);

        void ProcessKey(char key);

        void ProcessBackspace();

        void ProcessTimeout();

        void ProcessCleaning();
    }
}
