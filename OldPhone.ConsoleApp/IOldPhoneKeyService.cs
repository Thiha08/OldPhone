namespace OldPhone.ConsoleApp
{
    public interface IOldPhoneKeyService
    {
        string CurrentText { get; }

        void ProcessKey(char key);

        void ProcessBackspace();

        void ProcessTimeout();

        void ProcessComplete();

        void ProcessCleaning();
    }
}
