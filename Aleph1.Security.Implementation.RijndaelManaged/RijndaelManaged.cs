using Aleph1.Security.Contracts;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aleph1.Security.Implementation.RijndaelManagedCipher
{
    /// <summary>Implementation of the ICyper interface using the RijndaelManaged algorithm</summary>
    /// <seealso cref="ICipher" />
    public class RijndaelManagedCipher : ICipher
    {
        // This constant is used to determine the key size of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KEY_SIZE = 256;
        private const int KEY_SIZE_BYTES = KEY_SIZE / 8;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DERIVATION_ITERATIONS = 1000;

        private class Storage<T>
        {
            public Storage() { }
            public Storage(T data, string appPrefix, string userUniqueID, TimeSpan? timeSpan)
            {
                Data = data;
                AppPrefix = appPrefix;
                UserUniqueID = userUniqueID;
                if (timeSpan.HasValue)
                {
                    ExpirationDate = DateTime.UtcNow + timeSpan.Value;
                }
            }

            public T Data { get; set; }

            /// <summary>if the date is null - the ticket is unlimited</summary>
            public DateTime? ExpirationDate { get; set; }

            /// <summary>Encrypted into the ticket for further check</summary>
            public string AppPrefix { get; set; }

            /// <summary>Encrypted into the ticket for further check</summary>
            public string UserUniqueID { get; set; }
        }

        private static byte[] GenerateBitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[KEY_SIZE_BYTES];

            // Fill the array with cryptographically secure random bytes.
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        /// <summary>Decrypts the specified data for a specific user in a specific application.</summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="appPrefix">The application prefix.</param>
        /// <param name="userUniqueID">The user unique identifier.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>the decrypted data</returns>
        public T Decrypt<T>(string appPrefix, string userUniqueID, string encryptedData)
        {
            string passPhrase = appPrefix + userUniqueID;

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(encryptedData);

            // Get the salt bytes by extracting the first KEY_SIZE_BYTES bytes from the supplied cipherText bytes.
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEY_SIZE_BYTES).ToArray();

            // Get the IV bytes by extracting the next KEY_SIZE_BYTES bytes from the supplied cipherText bytes.
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEY_SIZE_BYTES).Take(KEY_SIZE_BYTES).ToArray();

            // Get the actual cipher text bytes by removing the first 2 * KEY_SIZE_BYTES bytes from the cipherText string.
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(KEY_SIZE_BYTES * 2).Take(cipherTextBytesWithSaltAndIv.Length - (KEY_SIZE_BYTES * 2)).ToArray();

            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
            {
                byte[] keyBytes = password.GetBytes(KEY_SIZE_BYTES);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = KEY_SIZE;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();

                                //Throws error when invalid
                                string serizlizedTicket = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                                Storage<T> store = JsonConvert.DeserializeObject<Storage<T>>(serizlizedTicket);

                                //Ticket Decrypted successfully - but not for the right user
                                if (store.AppPrefix != appPrefix || store.UserUniqueID != userUniqueID)
                                {
                                    throw new CryptographicException($"Ticket Violation");
                                }

                                //Ticket Decrypted successfully - but expired
                                if (store.ExpirationDate.HasValue && store.ExpirationDate.Value < DateTime.UtcNow)
                                {
                                    throw new CryptographicException($"Data Expired {DateTime.UtcNow - store.ExpirationDate.Value} ago");
                                }

                                return store.Data;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Encrypts the specified data for a specific user in a specific application.</summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="appPrefix">The application prefix.</param>
        /// <param name="userUniqueID">The user unique identifier.</param>
        /// <param name="data">The data to encrypt</param>
        /// <param name="timeSpan">the duration of the ticket generated</param>
        /// <returns>the encrypted data</returns>
        public string Encrypt<T>(string appPrefix, string userUniqueID, T data, TimeSpan? timeSpan = null)
        {
            if (string.IsNullOrWhiteSpace(appPrefix) || string.IsNullOrWhiteSpace(userUniqueID))
            {
                throw new CryptographicException("empty appPrefix or userUniqueID");
            }

            Storage<T> store = new Storage<T>(data, appPrefix, userUniqueID, timeSpan);
            string plainText = JsonConvert.SerializeObject(store);
            string passPhrase = appPrefix + userUniqueID;

            // Salt and IV is randomly generated each time, but is prepended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            byte[] saltStringBytes = GenerateBitsOfRandomEntropy();
            byte[] ivStringBytes = GenerateBitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
            {
                byte[] keyBytes = password.GetBytes(KEY_SIZE_BYTES);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = KEY_SIZE;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                byte[] cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }
    }
}
