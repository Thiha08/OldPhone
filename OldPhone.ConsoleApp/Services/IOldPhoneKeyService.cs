namespace OldPhone.ConsoleApp.Services
{
    public interface IOldPhoneKeyService : IDisposable
    {
        string CurrentText { get; }

        event Action<string> TextChanged;

        string Process(string input);

        void ProcessKey(char key);

        void ProcessBackspace();

        void ProcessTimeout();

        void ProcessComplete();

        void ProcessCleaning();
    }
}
