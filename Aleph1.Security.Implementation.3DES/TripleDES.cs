using Aleph1.Security.Contracts;
using System;
using System.Text;
using System.Web.Script.Serialization;

namespace Aleph1.Security.Implementation._3DES
{
    /// <summary>Implementation of the ICyper interface using the TripleDES algorithm</summary>
    /// <seealso cref="ICipher" />
    public class TripleDES : ICipher
    {
        private const int KEY_SIZE = 24;
        private static readonly JavaScriptSerializer TEXT_SERIALIZER = new JavaScriptSerializer();

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
                this.Data = data;
                if (timeSpan.HasValue)
                {
                    this.ExpirationDate = DateTime.UtcNow + timeSpan.Value;
                }
            }

            public T Data { get; set; }
            
            /// <summary>if the date is null - the ticket is unlimited</summary>
            public DateTime? ExpirationDate { get; set; }
        }

        public T Decrypt<T>(string appPrefix, string userUniqueID, string encryptedData)
        {
            throw new NotImplementedException();
        }

        public string Encrypt<T>(string appPrefix, string userUniqueID, T data, TimeSpan? timeSpan = null)
        {
            throw new NotImplementedException();
        }
    }
}
