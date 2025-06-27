namespace OldPhone.ConsoleApp
{
    public static class Constants
    {
        // pause for a second in order to type two characters  
        public const int TIMER_INTERVAL_MS = 1000;

        // Regex pattern for valid phone keypad characters + special commands
        // [1-9] = digits 1-9
        // 0 = digit 0  
        // [\s] = space character
        // [*#] = special characters * and #
        // [MOSQ] = special commands M, S, Q
        public const string PHONE_KEY_REGEX = @"^[1-90*#MOQS\s]+$";
    }
}
