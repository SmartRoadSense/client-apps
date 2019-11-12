using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SmartRoadSense {

    /// <summary>
    /// Extensions for cryptographic data buffers.
    /// </summary>
    public static class CryptographicExtensions {

        /// <summary>
        /// Converts a string to its SHA-512 hash.
        /// The string is assumed to be encoded using UTF8.
        /// </summary>
        public static byte[] ToSha512Hash(this string data) {
            return ToSha512Hash(data, Encoding.UTF8);
        }

        /// <summary>
        /// Converts a string to its SHA-512 hash.
        /// </summary>
        public static byte[] ToSha512Hash(this string data, Encoding stringEncoding) {
            var shaAlgorithm = new SHA512Managed();
            return shaAlgorithm.ComputeHash(stringEncoding.GetBytes(data));
        }

        /// <summary>
        /// Computes the SHA-512 hash of a byte buffer.
        /// </summary>
        public static byte[] ToSha512Hash(this byte[] buffer) {
            var shaAlgorithm = new SHA512Managed();
            return shaAlgorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Computes the SHA-160 hash of a byte buffer.
        /// </summary>
        public static byte[] ToSha160Hash(this byte[] buffer) {
            var shaAlgorithm = new SHA1Managed();
            return shaAlgorithm.ComputeHash(buffer);
        }

        public static string ToBase64(this byte[] data) {
            return Convert.ToBase64String(data);
        }

        public static byte[] FromBase64(this string data) {
            return Convert.FromBase64String(data);
        }

        /// <summary>
        /// Performs a fast equality check between byte arrays.
        /// </summary>
        public static bool FastIsEquals(this byte[] a, byte[] b) {
            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; ++i) {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a raw data array to a readable hexadecimal string.
        /// </summary>
        public static string ToReadable(this byte[] data) {
            return string.Concat(from b in data select b.ToString("X2"));
        }

    }

}

