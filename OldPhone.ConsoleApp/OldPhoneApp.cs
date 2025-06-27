using OldPhone.ConsoleApp.Services;

namespace OldPhone.ConsoleApp
{
    public class OldPhoneApp
    {
        private readonly IOldPhoneKeyService _keyService;

        private bool _isSingleKeyMode = false;

        public OldPhoneApp(IOldPhoneKeyService keyService)
        {
            _keyService = keyService;
            _keyService.TextChanged += text => Console.WriteLine($"Text: {text}");
            _keyService.TextCompleted += text => Console.WriteLine($"OldPhonePad() Final output: {text}\n\n");
        }

        public void Run()
        {
            DisplayWelcomeMessage();
            DisplayCommands();
            
            while (true)
            {
                if (!_isSingleKeyMode)
                {
                    ProcessOldPhonePadInput();
                }
                else
                {
                    // Single key mode - wait for user input
                    var key = GetUserInput();
                    if (ProcessSingleKeyInput(key))
                        break;
                }
            }
        }

        private static char GetUserInput()
        {
             return PhoneCmdHelper.ReadValidPhoneKey();
        }

        private bool ProcessSingleKeyInput(char key)
        {
            if (ProcessSpecialCommand(key))
                return false;

            _keyService.OldPhonePad(key.ToString());
            return false;
        }

        private void ProcessOldPhonePadInput()
        {
            Console.WriteLine("Enter a string to process:");

            var input = PhoneCmdHelper.ReadValidPhoneInput();
            
            // Check for special commands at the beginning of the input
            if (input.Length > 0 && ProcessSpecialCommand(input[0]))
                return;
            
            var output = _keyService.OldPhonePad(input);
        }

        private bool ProcessSpecialCommand(char command)
        {
            switch (command)
            {
                case 'S':
                    _isSingleKeyMode = true;
                    Console.WriteLine("Switched to Single Key Input Mode\n");
                    return true;

                case 'O':
                    _isSingleKeyMode = false;
                    Console.WriteLine("Switched to String Input Mode\n");
                    return true;

                case 'M':
                    DisplayCommands();
                    return true;

                case 'Q':
                    Console.WriteLine("Exiting application...");
                    Environment.Exit(0);
                    return true;

                default:
                    return false;
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
            Console.WriteLine("\n*** Available Commands ***\n");
            Console.WriteLine("O: OldPhonePad(string input) - Default Mode");
            Console.WriteLine("S: Switch to Single Key Input Mode");
            Console.WriteLine("M: Display Commands\n\n");
            Console.WriteLine("1-9: Input keys");
            Console.WriteLine("0: Space");
            Console.WriteLine("*: Backspace");
            Console.WriteLine("#: End input");
            Console.WriteLine("Q: Quit");
            Console.WriteLine("\n**************************\n");
        }
    }
}
