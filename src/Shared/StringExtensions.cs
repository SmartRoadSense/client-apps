using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartRoadSense.Shared {

    public static class StringExtensions {

        /// <summary>
        /// Converts a word to title case.
        /// </summary>
        /// <remarks>
        /// Does not respect acronyms and full uppercase words.
        /// </remarks>
        public static string ToTitleCase(this string s) {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            var sb = new StringBuilder(s.Length);

            bool waitingForWord = true;
            foreach (var c in s) {
                if (waitingForWord && Char.IsLetterOrDigit(c)) {
                    //Found first letter of word
                    sb.Append(Char.ToUpperInvariant(c));
                    waitingForWord = false;
                }
                else {
                    sb.Append(Char.ToLowerInvariant(c));

                    if (Char.IsWhiteSpace(c)) {
                        waitingForWord = true;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Capitalizes a string.
        /// </summary>
        public static string Capitalize(this string s) {
            if (s == null)
                throw new ArgumentNullException();
            if (s.Length == 0)
                return string.Empty;

            return Char.ToUpper(s[0]) + s.Substring(1);
        }

        private const RegexOptions DefaultRegexOptions =
#if !WINDOWS_PHONE_APP
            RegexOptions.Compiled |
#endif
            RegexOptions.Multiline | RegexOptions.CultureInvariant;

        private static Regex _regexHtml = new Regex(@"<[\w\s]*/?[\w\s]*>", DefaultRegexOptions);

        /// <summary>
        /// Strips all valid HTML tags from a string.
        /// </summary>
        public static string StripHtml(this string s) {
            return _regexHtml.Replace(s, string.Empty);
        }

        private static Regex _regexNewlines = new Regex(@"<[\s]*br[\s]*/?[\s]*>", DefaultRegexOptions);

        /// <summary>
        /// Replaces all HTML breaks with newlines.
        /// </summary>
        public static string HtmlBreaksToNewlines(this string s) {
            return _regexNewlines.Replace(s, "\n");
        }

#if __IOS__
        /// <summary>
        /// Prepares a localizes string to be displayed by a label in iOS.
        /// </summary>
        public static string PrepareForLabel(this string s) {
            return s.HtmlBreaksToNewlines().StripHtml();
        }
#endif

    }

}
