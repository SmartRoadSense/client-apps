using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Logging facility.
    /// </summary>
    public static class Log {

        public enum LogLevel {
            Debug,
            Warning,
            Error
        }

        private static string GetCallerMethod() {
#if WINDOWS_PHONE_APP
            //TODO
            return "Unknown method";
#else
            StackTrace st = new StackTrace();
            var frame = st.GetFrame(3);
            if (frame == null)
                return "Unknown stack";

            var method = frame.GetMethod();
            if (method == null)
                return "Unknown method";

            if (method.DeclaringType.DeclaringType != null) {
                //Special case for nested classes, especially for async compiler-generated ones
                return string.Format("{0}+{1}.{2}", method.DeclaringType.DeclaringType.FullName, method.DeclaringType.Name, method.Name);
            }
            else {
                return string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            }
#endif
        }

        [Conditional("DEBUG")]
        private static void InternalLog(LogLevel level, string message, Exception ex) {
#if __ANDROID__
            Java.Lang.Throwable throwable = null;
            if(ex != null) {
                throwable = new Java.Lang.Throwable(ex.Message);
            }

            switch(level) {
                case LogLevel.Debug:
                default:
                    global::Android.Util.Log.Debug(GetCallerMethod(), message);
                    break;

                case LogLevel.Warning:
                    global::Android.Util.Log.Warn(GetCallerMethod(), throwable, message);
                    break;

                case LogLevel.Error:
                    global::Android.Util.Log.Error(GetCallerMethod(), throwable, message);
                    break;
            }
#elif __IOS__
            Console.WriteLine("{0} {1} {2}", level, GetCallerMethod(), message);
            if(ex != null) {
                Console.WriteLine(ex);
            }
#elif WINDOWS_PHONE_APP
            System.Diagnostics.Debug.WriteLine("{0} {1} {2}", level, GetCallerMethod(), message);
            if(ex != null) {
                System.Diagnostics.Debug.WriteLine(ex);
            }
#elif DESKTOP
            //Swallow
#else
#error Unrecognized platform
#endif
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args) {
            InternalLog(LogLevel.Debug, string.Format(format, args), null);
        }

        public static void Warning(Exception ex, string format, params object[] args) {
            InternalLog(LogLevel.Warning, string.Format(format, args), ex);

#if !DEBUG && !DESKTOP
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex,
                new Dictionary<string, string> {
                    { "message", string.Format(format, args) }
                }
            );
#endif
        }

        public static void Error(Exception ex, string format, params object[] args) {
            InternalLog(LogLevel.Error, string.Format(format, args), ex);

#if !DEBUG && !DESKTOP
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex,
                new Dictionary<string, string> {
                    { "message", string.Format(format, args) }
                }
            );
#endif
        }

        /// <summary>
        /// Records an event and logs it remotely, if possible.
        /// </summary>
        public static void Event(string code, params string[] properties) {
            var propDic = new Dictionary<string, string>(properties.Length / 2);
            for(int i = 0; (i + 1) < properties.Length; i += 2) {
                propDic[properties[i]] = properties[i + 1];
            }

            Event(code, propDic);
        }

        /// <summary>
        /// Records an event and logs it remotely, if possible.
        /// </summary>
        public static void Event(string code, Dictionary<string, string> properties) {
#if DEBUG
            InternalLog(LogLevel.Debug, "Event: " + code.ToUpperInvariant(), null);
#endif

#if !DEBUG && !DESKTOP
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(code, properties);
#endif
        }

    }

}
