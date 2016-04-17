using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecuritySystemService.Helpers
{
    public static class DynamicHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DynamicHelper));

        public static Dictionary<string, string> ToStringDictionary(this object obj)
        {
            if (obj == null)
                return new Dictionary<string, string>();
            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(pi => pi.CanRead).ToArray();

            var argsToUse = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Where(pi => pi.CanRead)
               .ToDictionary(pi => pi.Name, pi => pi.GetValue(obj)?.ToString());
            return argsToUse;
        }
    }
}
