using System.Collections.ObjectModel;
using System.Timers;
using Timer = System.Timers.Timer;

namespace OldPhone.ConsoleApp
{
    internal class Program
    {
        private static Timer _pauseTimer;

        private static ReadOnlyDictionary<char, string> _keyMap;

        public static event Action<string> TextChanged;

        private static char _currentKey = '\0';

        private static int _pressCount = 0;

        private static string _text = "";

        public static void Main(string[] args)
        {
            _keyMap = KeyMap.GetDefaultKeyMap();

            TextChanged += text => Console.WriteLine($"Text: {text}");

            SetPauseTimer();

            RunApplication();
        }


        private static void SetPauseTimer()
        {
            _pauseTimer = new Timer(1000); // pause for a second in order to type two characters
            _pauseTimer.Elapsed += OnTimedEvent;
            _pauseTimer.AutoReset = false;
        }

        private static void RunApplication()
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
            Console.WriteLine("Q: Quit");
            Console.WriteLine("**************************");
        }

        private static char GetUserInput()
        {
            var key = Console.ReadKey(true);
            Console.WriteLine();
            return key.KeyChar;
        }

        private static bool ProcessUserInput(Char key)
        {
            switch (key)
            {
                case '*': // * key
                    HandleBackspace();
                    return false;

                case '#': // # key
                    HandleCompleteInput();
                    return false;

                case 'Q':
                    return true; // Exit the application

                default:
                    HandleKeyPress(key);
                    return false;
            }
        }

        private static void HandleBackspace()
        {
            if (_text.Length > 0)
            {
                _text = _text[..^1];
                ResetCurrentKey();
                TextChanged?.Invoke(_text);
            }
        }

        private static void HandleCompleteInput()
        {
            if (_text.Length > 0)
            {
                Console.WriteLine($"Final output: {_text}");
                _text = "";
                ResetCurrentKey();
                TextChanged?.Invoke(_text);
            }
            else
            {
                Console.WriteLine("No output to complete.");
            }
        }

        private static void HandleKeyPress(char key)
        {
            if (!_keyMap.ContainsKey(key))
            {
                Console.WriteLine("Invalid key pressed.");
                DisplayCommands();
                return;
            }

            _pauseTimer.Stop();

            if (key == _currentKey)
            {
                // Same key - cycle to next character
                _pressCount = (_pressCount + 1) % _keyMap[key].Length;
                ReplaceLastCharacter(_keyMap[key][_pressCount]);
            }
            else
            {
                // New key - start fresh sequence
                _currentKey = key;
                _pressCount = 0;
                AddCharacter(_keyMap[key][0]);
            }

            _pauseTimer.Start();
        }

        private static void AddCharacter(char c)
        {
            _text += c;
            TextChanged?.Invoke(_text);
        }

        private static void ReplaceLastCharacter(char c)
        {
            if (_text.Length > 0)
            {
                _text = _text[..^1] + c;
                TextChanged?.Invoke(_text);
            }
        }

        private static void ResetCurrentKey()
        {
            _currentKey = '\0';
            _pressCount = 0;
        }

        private static void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            // Handle timeout logic here 
            ResetCurrentKey();
        }

        public static void Dispose()
        {
            _pauseTimer?.Dispose();
        }

    }
}
