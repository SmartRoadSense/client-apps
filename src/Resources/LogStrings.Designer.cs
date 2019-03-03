﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartRoadSense.Resources {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class LogStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LogStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SmartRoadSense.Resources.LogStrings", typeof(LogStrings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating internal database after update.
        /// </summary>
        public static string DatabaseMigrationReset {
            get {
                return ResourceManager.GetString("DatabaseMigrationReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Synchronization failed: {0}.
        /// </summary>
        public static string FileUploadFailure {
            get {
                return ResourceManager.GetString("FileUploadFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Synchronization completed ({0} chunks uploaded).
        /// </summary>
        public static string FileUploadSummaryPlural {
            get {
                return ResourceManager.GetString("FileUploadSummaryPlural", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Synchronization completed (1 chunk uploaded).
        /// </summary>
        public static string FileUploadSummarySingular {
            get {
                return ResourceManager.GetString("FileUploadSummarySingular", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to write data file.
        /// </summary>
        public static string FileWriteError {
            get {
                return ResourceManager.GetString("FileWriteError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New data written to file.
        /// </summary>
        public static string FileWriteSuccess {
            get {
                return ResourceManager.GetString("FileWriteSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GPS disabled.
        /// </summary>
        public static string GpsDisabled {
            get {
                return ResourceManager.GetString("GpsDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GPS enabled.
        /// </summary>
        public static string GpsEnabled {
            get {
                return ResourceManager.GetString("GpsEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GPS got satellite fix.
        /// </summary>
        public static string GpsFix {
            get {
                return ResourceManager.GetString("GpsFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GPS fix lost.
        /// </summary>
        public static string GpsFixLost {
            get {
                return ResourceManager.GetString("GpsFixLost", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internal computation error.
        /// </summary>
        public static string InternalEngineError {
            get {
                return ResourceManager.GetString("InternalEngineError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recording ignored in offline mode.
        /// </summary>
        public static string RecordingOffline {
            get {
                return ResourceManager.GetString("RecordingOffline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Started recording.
        /// </summary>
        public static string RecordingStarted {
            get {
                return ResourceManager.GetString("RecordingStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stopped recording.
        /// </summary>
        public static string RecordingStopped {
            get {
                return ResourceManager.GetString("RecordingStopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recording suspended because user movement was too slow.
        /// </summary>
        public static string RecordingSuspendedSpeed {
            get {
                return ResourceManager.GetString("RecordingSuspendedSpeed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recording suspended because the device remained stationary for too long.
        /// </summary>
        public static string RecordingSuspendedStationary {
            get {
                return ResourceManager.GetString("RecordingSuspendedStationary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SmartRoadSense v{0} started.
        /// </summary>
        public static string Started {
            get {
                return ResourceManager.GetString("Started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:0.0} km track recorded ({1} data points).
        /// </summary>
        public static string StatsRecorded {
            get {
                return ResourceManager.GetString("StatsRecorded", resourceCulture);
            }
        }
    }
}
