namespace OldPhone.ConsoleApp.Services
{
    public interface IOldPhoneKeyService : IDisposable
    {
        string CurrentText { get; }

        event Action<string> TextChanged;

        void ProcessKey(char key);

        void ProcessBackspace();

        void ProcessTimeout();

        void ProcessComplete();

        void ProcessCleaning();
    }
}
