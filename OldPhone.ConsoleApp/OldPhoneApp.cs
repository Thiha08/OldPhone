using OldPhone.ConsoleApp.Services;

namespace OldPhone.ConsoleApp
{
    public class OldPhoneApp
    {
        private readonly IOldPhoneKeyService _keyService;

        public OldPhoneApp(IOldPhoneKeyService keyService)
        {
            _keyService = keyService;
            _keyService.TextChanged += text => Console.WriteLine($"Text: {text}");
        }

        public void Run()
        {
            DisplayWelcomeMessage();
            DisplayCommands();

            while (true)
            {
                var key = GetUserInput();

                if (ProcessUserInput(key))
                    break;
            }
        }

        private static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("*** OldPhone Keypad System ***\n");
            Console.WriteLine("Application ready! Press any key to start...");
            Console.ReadKey();
            Console.Clear();
        }

        private static void DisplayCommands()
        {
            Console.WriteLine("*** Available Commands ***");
            Console.WriteLine("1-9/0: Input keys");
            Console.WriteLine("*: Backspace");
            Console.WriteLine("#: End input");
            Console.WriteLine("O: OldPhonePad(string input) ");
            Console.WriteLine("Q: Quit");
            Console.WriteLine("**************************");
        }

        private static char GetUserInput()
        {
            var key = Console.ReadKey(true);
            Console.WriteLine();
            return key.KeyChar;
        }

        private bool ProcessUserInput(char key)
        {
            switch (key)
            {
                case 'O':
                    ProcessOldPhonePadInput();
                    return false;

                case '*':
                    _keyService.ProcessBackspace();
                    return false;

                case '#':
                    ProcessEndInput();
                    return false;

                case 'Q':
                    // Exit the application
                    return true;

                default:
                    _keyService.ProcessKey(key);
                    return false;
            }
        }

        private void ProcessOldPhonePadInput()
        {
            Console.WriteLine("Enter a string to process:");
            var input = Console.ReadLine() ?? string.Empty;
            var output = _keyService.Process(input);
            Console.WriteLine($"OldPhonePad(\"{input}\") => output: {output} \n\n");
            _keyService.ProcessCleaning();
            DisplayCommands();
        }

        private void ProcessEndInput()
        {
            Console.WriteLine($"Final output: {_keyService.CurrentText} \n\n");
            _keyService.ProcessCleaning();
            DisplayCommands();
        }

    }
}
