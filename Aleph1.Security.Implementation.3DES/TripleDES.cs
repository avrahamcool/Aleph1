using Aleph1.Security.Contracts;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Aleph1.Security.Implementation._3DES
{
    /// <summary>Implementation of the ICyper interface using the TripleDES algorithm</summary>
    /// <seealso cref="ICipher" />
    public class TripleDES : ICipher
    {
        private const int KEY_SIZE = 24;

        private static byte[] GetKey(string appPrefix, string userUniqueID)
        {
            string key = appPrefix + userUniqueID;
            string monoSizeKey = Encoding.UTF8.GetByteCount(key) < KEY_SIZE
                ? key.PadRight(KEY_SIZE, ' ')
                : key.Substring(0, KEY_SIZE);
            return Encoding.UTF8.GetBytes(monoSizeKey);
        }

        private class Storage<T>
        {
            public Storage() { }
            public Storage(T data, TimeSpan? timeSpan)
            {
                Data = data;
                if (timeSpan.HasValue)
                    ExpirationDate = DateTime.UtcNow + timeSpan.Value;
            }

            public T Data { get; set; }
            
            /// <summary>if the date is null - the ticket is unlimited</summary>
            public DateTime? ExpirationDate { get; set; }
        }

        /// <summary>Decrypts the specified data for a specific user in a spesific application.</summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="appPrefix">The application prefix.</param>
        /// <param name="userUniqueID">The user unique identifier.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>the decrypted data</returns>
        /// <exception cref="CryptographicException"></exception>
        public T Decrypt<T>(string appPrefix, string userUniqueID, string encryptedData)
        {
            using (TripleDESCryptoServiceProvider cryptoService = new TripleDESCryptoServiceProvider())
            {
                cryptoService.Mode = CipherMode.ECB;
                cryptoService.Padding = PaddingMode.PKCS7;
                cryptoService.Key = GetKey(appPrefix, userUniqueID);
                using (ICryptoTransform decryptor = cryptoService.CreateDecryptor())
                {
                    byte[] buffer = Convert.FromBase64String(encryptedData.Replace(' ', '+'));

                    //Throws error when invalid
                    string serizlizedTicket = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length));
                    
                    Storage<T> store = JsonConvert.DeserializeObject<Storage<T>>(serizlizedTicket);
                    if (store.ExpirationDate.HasValue && store.ExpirationDate.Value < DateTime.UtcNow)
                        throw new CryptographicException($"Data Expired {DateTime.UtcNow - store.ExpirationDate.Value} ago");

                    return store.Data;
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
            using (TripleDESCryptoServiceProvider cryptoService = new TripleDESCryptoServiceProvider())
            {
                cryptoService.Mode = CipherMode.ECB;
                cryptoService.Padding = PaddingMode.PKCS7;
                cryptoService.Key = GetKey(appPrefix, userUniqueID);

                using (ICryptoTransform encryptor = cryptoService.CreateEncryptor())
                {
                    Storage<T> store = new Storage<T>(data, timeSpan);
                    byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(store));
                    return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
                }
            }
        }
    }
}
