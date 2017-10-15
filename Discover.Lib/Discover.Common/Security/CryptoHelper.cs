using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Discover.Security
{
    public class CryptoHelper
    {
        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <remarks>Assumes the hash value is to be generated using the SHA1Managed algorithm</remarks>
        /// <param name="plainText">Plaintext value to be hashed.</param>
        /// <returns>Hash value formatted as a base64-encoded string.</returns>
        public static string ComputeHash(string plainText)
        {
            return ComputeHash(plainText, new SHA1Managed(), null);
        }

        /// <summary>
        /// Generates a hash for the given plain text value and returns a base64-encoded result.
        /// <para></para>
        /// Before the hash is computed, a random salt is generated and appended to the plain text. 
        /// This salt is stored at the end of the hash value, so it can be used later for hash verification.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be hashed.
        /// </param>
        /// <param name="hashAlgorithm">
        /// The algorithm to use for computing the hash.
        /// </param>
        /// <param name="saltBytes">
        /// Salt bytes. If this parameter is null, a random salt value will be generated.
        /// </param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string ComputeHash(string plainText, HashAlgorithm hashAlgorithm, byte[] saltBytes)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            // If salt is not specified, generate some
            if (saltBytes == null)
            {
                saltBytes = new byte[new Random().Next(4, 8)];
                new RNGCryptoServiceProvider().GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            var plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];
            Array.Copy(plainTextBytes, plainTextWithSaltBytes, plainTextBytes.Length);
            Array.Copy(saltBytes, 0, plainTextWithSaltBytes, plainTextBytes.Length, saltBytes.Length);
            
            // Compute hash value of plain text with appended salt.
            var hashBytes = hashAlgorithm.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            var hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];
            Array.Copy(hashBytes, hashWithSaltBytes, hashBytes.Length);
            Array.Copy(saltBytes, 0, hashWithSaltBytes, hashBytes.Length, saltBytes.Length);

            // Return the result as a base64-encoded string
            return Convert.ToBase64String(hashWithSaltBytes);
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash value. Plain text is hashed with the same salt value as the original hash.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="hashValue"></param>
        /// <remarks>Assumes the hash value was generated using the SHA1Managed algorithm</remarks>
        /// <returns></returns>
        public static bool VerifyHash(string plainText, string hashValue)
        {
            return VerifyHash(plainText, hashValue, new SHA1Managed());
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash value.
        /// <para></para>
        /// Plain text is hashed with the same salt value as the original hash (i.e. the salt value embedded by the ComputeHash method).
        /// </summary>
        /// <param name="plainText">
        /// Plain text to be verified against the specified hash.
        /// </param>
        /// <param name="hashValue">
        /// Base64-encoded hash value produced by ComputeHash function. This value includes the original salt appended to it.
        /// </param>
        /// <param name="hashAlgorithm">
        /// The algorithm to use for computing the hash.
        /// </param>
        /// <returns>
        /// True if the computed hash for the given plain-text matches the given hashValue, otherwise False
        /// </returns>
        public static bool VerifyHash(string plainText, string hashValue, HashAlgorithm hashAlgorithm)
        {
            if (string.IsNullOrEmpty(plainText)) return false;

            // Convert base64-encoded hash value into a byte array.
            var hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // Convert size of hash from bits to bytes.
            int hashSizeInBytes = hashAlgorithm.HashSize / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes) return false;

            // Extract original salt bytes from hash.
            var saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];
            Array.Copy(hashWithSaltBytes, hashSizeInBytes, saltBytes, 0, saltBytes.Length);
            
            // Compute a new hash string.
            string expectedHashString = ComputeHash(plainText, hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash, the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        public const string RandomPasswordChars = "23456789BCDFGHJKLMNPQRSTVWXYZbcdfghjkmnpqrstvwxyz";

        /// <summary>
        /// Generates a random password of a given length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomPassword(int length)
        {
            var result = new StringBuilder(length);
            var rng = new Random();

            for (var i = 0; i < length; i++)
            {
                result.Append(RandomPasswordChars[rng.Next(0, RandomPasswordChars.Length - 1)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a random password of a given length, using the given sets of characters, including at least one character from each set
        /// </summary>
        /// <param name="length"></param>
        /// <param name="characterSets"></param>
        /// <returns></returns>
        public static string GenerateRandomPassword(int length, params string[] characterSets)
        {
            if (characterSets.Length == 0) throw new ArgumentException("At least one character set must be supplied");

            var result = new StringBuilder(length);
            var rng = new Random();
            string characterSet = null;

            // include at least one character from each character set
            for (var i = 0; i < characterSets.Length; i++)
            {
                characterSet = characterSets[i];
                result.Append(characterSet[rng.Next(0, characterSet.Length - 1)]);
            }

            // fill the rest of the string with randomly selected characters from randomly selected character sets
            for (var i = characterSets.Length; i < length; i++)
            {
                characterSet = characterSets[rng.Next(0, characterSets.Length - 1)];
                result.Append(characterSet[rng.Next(0, characterSet.Length - 1)]);
            }

            return result.ToString();
        }
    }
}
