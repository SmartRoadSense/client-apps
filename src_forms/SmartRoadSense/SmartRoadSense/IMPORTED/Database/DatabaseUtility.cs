﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SmartRoadSense
{

    /// <summary>
    /// Performs database initialization and migration.
    /// </summary>
    public static class DatabaseUtility
    {

        public const int TargetDataVersion = 2;

        /// <summary>
        /// Initializes the database.
        /// </summary>
        public static async Task Initialize()
        {
            //Explicitly set internal SQLite provider
            //See: https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-net-and-Android-N
            /*
            #if __IOS__
                        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
            #else
                        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            #endif
            */

            var currentVersion = SettingsManager.Instance.DataVersion;
            if (currentVersion < 0) {
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

            SettingsManager.Instance.DataVersion = TargetDataVersion;

            Log.Debug("Database initialized");
        }

        private static async Task FullInstallation() {
            // Remove previous (unversioned) versions of the database, if any
            if(await FileOperations.CheckFile(FileNaming.DatabasePath)) {
                UserLog.Add(AppResources.DatabaseMigrationReset);

                Log.Debug("Deleting old database: bye bye existing data");
                Log.Event("Database.reset");
                var token = await FileOperations.GetToken(FileNaming.DatabasePath);
                await token.Delete();
            }

            // Database creation
            using (var db = OpenConnection()) {
                db.CreateTable<TrackUploadRecord>();
                db.CreateTable<StatisticRecord>();
            }

            Log.Event("Database.create");
        }

        private static void Migrate(int currentVersion)
        {
            // Not yet needed
        }

        /// <summary>
        /// Opens a new connection to the database.
        /// </summary>
        public static SQLiteConnection OpenConnection() {
            return new SQLiteConnection(
                FileNaming.DatabasePath
            );
        }

        //public static SQLiteAsyncConnection OpenConnection()
        //{
        //    return new SQLiteAsyncConnection(FileNaming.DatabasePath);
        //}
    }

}