using System.Collections.ObjectModel;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace OldPhone.ConsoleApp.Services
{
    public class OldPhoneKeyService : IOldPhoneKeyService, IDisposable
    {
        private readonly ReadOnlyDictionary<char, string> _keyMap;
        private readonly Timer _pauseTimer;
        private readonly StringBuilder _textBuilder = new StringBuilder();

        private char _currentKey = '\0';
        private int _pressCount = 0;
        private bool _disposed = false;

        public string CurrentText => _textBuilder.ToString();

        public event Action<string> TextChanged = delegate { };
        public event Action<string> TextCompleted = delegate { };
        
        public OldPhoneKeyService()
        {
            _keyMap = KeyMap.GetDefaultKeyMap();
            _pauseTimer = new Timer(Constants.TIMER_INTERVAL_MS);
            _pauseTimer.Elapsed += OnTimedEvent;
            _pauseTimer.AutoReset = false;
        }

        /// <summary>
        /// Coding Challenge: Implement the OldPhonePad method to simulate the old phone keypad input.
        /// </summary>
        public string OldPhonePad(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char key = input[i];

                switch (key)
                {
                    case '#':
                        ProcessComplete();
                        break;

                    case '*':
                        ProcessBackspace();
                        break;

                    case ' ':
                        ProcessTimeout();
                        break;

                    default:
                        ProcessKey(key);
                        break;
                }
            }

            return CurrentText;
        }

        public void ProcessKey(char key)
        {
            if (!_keyMap.ContainsKey(key))
                return;

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

        public void ProcessBackspace()
        {
            if (_textBuilder.Length > 0)
            {
                _textBuilder.Length--;
                ResetCurrentKey();
                TextChanged?.Invoke(CurrentText);
            }
        }

        public void ProcessCleaning()
        {
            _textBuilder.Clear();
            ResetCurrentKey();
        }

        public void ProcessTimeout()
        {
            ResetCurrentKey();
        }

        public void ProcessComplete()
        {
            TextCompleted?.Invoke(CurrentText);
            ProcessCleaning();
        }

        #region Private Methods  

        private void AddCharacter(char c)
        {
            _textBuilder.Append(c);
            TextChanged?.Invoke(CurrentText);
        }

        private void ReplaceLastCharacter(char c)
        {
            if (_textBuilder.Length > 0)
            {
                _textBuilder[^1] = c;
                TextChanged?.Invoke(CurrentText);
            }
        }

        private void ResetCurrentKey()
        {
            _currentKey = '\0';
            _pressCount = 0;
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            ProcessTimeout();
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _pauseTimer?.Dispose();
                _disposed = true;
            }
        }
    }
}
