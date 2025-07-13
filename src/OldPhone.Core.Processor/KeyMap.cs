using System.Collections.ObjectModel;

namespace OldPhone.Core.Processor
{
    public static class KeyMap
    {
        public static ReadOnlyDictionary<char, string> GetDefaultKeyMap()
        {
            var dict = new Dictionary<char, string>
            {
                ['1'] = "&'(",
                ['2'] = "ABC",
                ['3'] = "DEF",
                ['4'] = "GHI",
                ['5'] = "JKL",
                ['6'] = "MNO",
                ['7'] = "PQRS",
                ['8'] = "TUV",
                ['9'] = "WXYZ",
                ['0'] = " ",
            };

            return new ReadOnlyDictionary<char, string>(dict);
        }
    }
}
