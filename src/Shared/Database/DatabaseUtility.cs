using SmartRoadSense.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared.Database {

    /// <summary>
    /// Performs database initialization and migration.
    /// </summary>
    public static class DatabaseUtility {

        public const int TargetDataVersion = 3;

        /// <summary>
        /// Initializes the database.
        /// </summary>
        public static async Task Initialize() {
            //Explicitly set internal SQLite provider
            //See: https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-net-and-Android-N
#if __IOS__
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#else
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#endif
            var currentVersion = Settings.DataVersion;
            if(currentVersion < 0) {
                Log.Debug("No data version found, database installation required");
                await FullInstallation();
            }
            else if(currentVersion == TargetDataVersion) {
                Log.Debug("Database is already at target version v{0}", TargetDataVersion);
            }
            else {
                // Migration needed
                Migrate(currentVersion);
            }

            Settings.DataVersion = TargetDataVersion;

            Log.Debug("Database initialized");
        }

        private static async Task FullInstallation() {
            // Remove previous (unversioned) versions of the database, if any
            if(await FileOperations.CheckFile(FileNaming.DatabasePath)) {
                UserLog.Add(LogStrings.DatabaseMigrationReset);

                Log.Debug("Deleting old database: bye bye existing data");
                Log.Event("Database.reset");
                var token = await FileOperations.GetToken(FileNaming.DatabasePath);
                await token.Delete();
            }

            // Database creation
            using(var db = OpenConnection()) {
                db.CreateTable<TrackUploadRecord>();
                db.CreateTable<StatisticRecord>();
            }

            Log.Event("Database.create");
        }

        private static void Migrate(int currentVersion) {
            if(currentVersion <= 2) {
                // Drop index on StatisticRecord and re-create table (adds columns)
                using(var db = OpenConnection()) {
                    db.Execute("DROP INDEX IF EXISTS StatisticRecord_TrackId");
                    db.CreateTable<StatisticRecord>();
                }

                Log.Event("Database.migrate.2");
            }
        }

        /// <summary>
        /// Opens a new connection to the database.
        /// </summary>
        public static SQLite.SQLiteConnection OpenConnection() {
            return new SQLite.SQLiteConnection(
                FileNaming.DatabasePath
            );
        }

    }

}
