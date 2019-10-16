using System;
using System.IO;

namespace SmartRoadSense
{
    public static class DatabaseSettings
    {
        static readonly string DbName = "TodoSQLite.db3";
        public static string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

    }
}
