using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolPayloadBenchmark {

    public static class CryptographicExtensions {

        /// <summary>
        /// Converts a string to its SHA3 hash.
        /// The string is assumed to be encoded using UTF8.
        /// </summary>
        public static byte[] ToSha3Hash(this string data) {
            return ToSha3Hash(data, Encoding.UTF8);
        }

        /// <summary>
        /// Converts a string to its SHA3 hash.
        /// </summary>
        public static byte[] ToSha3Hash(this string data, Encoding stringEncoding) {
            var shaAlgorithm = new SHA512Managed();

            return shaAlgorithm.ComputeHash(stringEncoding.GetBytes(data));
        }

        /// <summary>
        /// Computes the SHA hash of a byte buffer.
        /// </summary>
        public static byte[] ToShaHash(this byte[] buffer) {
            var shaAlgorithm = new SHA512Managed();

            return shaAlgorithm.ComputeHash(buffer);
        }

        public static string ToBase64(this byte[] data) {
            return Convert.ToBase64String(data);
        }

        public static byte[] FromBase64(this string data) {
            return Convert.FromBase64String(data);
        }

    }

}
