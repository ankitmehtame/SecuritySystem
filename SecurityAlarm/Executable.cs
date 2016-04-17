using System;
using System.IO;

namespace SecuritySystem.SecurityAlarm
{
    public static class Executable
    {
        public static string GetPath(Type refType = null)
        {
            if(refType == null)
            {
                refType = typeof(Executable);
            }
            var assemblyPath = new Uri(typeof(Executable).Assembly.CodeBase).LocalPath;
            return assemblyPath;
        }

        public static string GetDirectory(Type refType = null)
        {
            var path = GetPath(refType);
            return path == null ? null : Path.GetDirectoryName(path);
        }
    }
}
