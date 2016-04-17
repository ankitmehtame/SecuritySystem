using System;
using System.Linq;

namespace SecuritySystem.SecurityAlarm
{
    public class ArgumentsHandler
    {
        private readonly string[] _args;

        public ArgumentsHandler(string[] args)
        {
            _args = args;
        }

        public string GetArgumentValue(string argumentName)
        {
            var index = GetIndex(argumentName);
            if (index < 0)
            {
                return null;
            }
            var nextIndex = index + 1;
            if (!IsIndexWithinRange(nextIndex, _args.Length))
            {
                return null;
            }
            return _args[nextIndex];
        }

        public bool HasArgument(string argumentName)
        {
            return GetIndex(argumentName) > -1;
        }

        public int GetIndex(string argumentName)
        {
            var res = _args.Where((a, i) => string.Equals(argumentName, a, StringComparison.OrdinalIgnoreCase)).Select((a, i) => i).ToArray();
            if (!res.Any())
            {
                return -1;
            }
            return res.First();
        }

        private static bool IsIndexWithinRange(int index, int count)
        {
            return index >= 0 && index + 1 <= count;
        }
    }
}
