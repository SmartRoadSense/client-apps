using System;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Cryptography handler.
    /// </summary>
    public static class Crypto {

        /// <summary>
        /// The length of the secret in bytes.
        /// </summary>
        public const int SecretLength = 1024 / 8;

        /// <summary>
        /// The length of the hashed secret in bytes.
        /// </summary>
        public const int SecretHashLength = 512 / 8;

        /// <summary>
        /// The length of the track identifier in bytes.
        /// </summary>
        public const int TrackIdLength = 160 / 8;

        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a new secret.
        /// </summary>
        public static byte[] GenerateSecret() {
            var secret = new byte[SecretLength];
            _random.NextBytes(secret);

            return secret;
        }

    }

}

