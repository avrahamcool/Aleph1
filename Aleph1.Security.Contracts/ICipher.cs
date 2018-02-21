using System;

namespace Aleph1.Security.Contracts
{
    /// <summary>A common iterface for Encryption / Decryption of data</summary>
    public interface ICipher
    {
        /// <summary>Encrypts the specified data for a specific user in a specific application.</summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="appPrefix">The application prefix.</param>
        /// <param name="userUniqueID">The user unique identifier.</param>
        /// <param name="data">The data to encrypt</param>
        /// <param name="timeSpan">the duration of the ticket generated</param>
        /// <returns>the encrypted data</returns>
        string Encrypt<T>(string appPrefix, string userUniqueID, T data, TimeSpan? timeSpan = null);


        /// <summary>Decrypts the specified data for a specific user in a spesific application.</summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="appPrefix">The application prefix.</param>
        /// <param name="userUniqueID">The user unique identifier.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>the decrypted data</returns>
        T Decrypt<T>(string appPrefix, string userUniqueID, string encryptedData);
    }
}
