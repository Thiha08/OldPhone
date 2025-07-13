using OldPhone.Core.Processor;
using System.Text;
using System.Text.RegularExpressions;

namespace OldPhone.UI.ConsoleApp
{
    public static class PhoneCmdHelper
    {
        private static readonly Regex PhoneKeypadPattern = new Regex(Constants.PHONE_KEY_REGEX, RegexOptions.Compiled);

        /// <summary>
        /// Reads input from console
        /// </summary>
        /// <returns>The valid input string</returns>
        public static string ReadValidPhoneInput()
        {
            var input = new StringBuilder();
            
            while (true)
            {
                var key = Console.ReadKey(true);
                var keyChar = key.KeyChar;
                
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }
                
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input.Length--;
                        Console.Write("\b \b");
                    }
                    continue;
                }
                
                if (IsValidPhoneKeypadChar(keyChar))
                {
                    input.Append(keyChar);
                    Console.Write(keyChar);
                }

                // Invalid key - ignore it
            }
        }

        /// <summary>
        /// Reads a single valid phone keypad character from console
        /// </summary>
        /// <returns>The valid character</returns>
        public static char ReadValidPhoneKey()
        {
            while (true)
            {
                var key = Console.ReadKey(true);
                var keyChar = key.KeyChar;
                
                if (IsValidPhoneKeypadChar(keyChar))
                {
                    Console.WriteLine(keyChar);
                    return keyChar;
                }
                else
                {
                    // Invalid key - ignore it and continue waiting
                    Console.Write("\b \b");
                }
            }
        }

        public static bool IsValidPhoneKeypadChar(char c)
        {
            return PhoneKeypadPattern.IsMatch(c.ToString());
        }

        public static string FilterValidCharacters(string input)
        {
            return new string(input.Where(IsValidPhoneKeypadChar).ToArray());
        }
    }
}
