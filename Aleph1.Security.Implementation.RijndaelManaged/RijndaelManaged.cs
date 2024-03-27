using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Aleph1.Security.Contracts;

using Newtonsoft.Json;

namespace Aleph1.Security.Implementation.RijndaelManagedCipher
{
	/// <summary>Implementation of the ICyper interface using the RijndaelManaged algorithm</summary>
	/// <seealso cref="ICipher" />
	public class RijndaelManagedCipher : ICipher
	{
		private const int BLOCK_SIZE = 128;
		private const int BLOCK_SIZE_BYTES = BLOCK_SIZE / 8;

		private const int KEY_SIZE = 256;

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

#if NET48
		private static byte[] GenerateBitsOfRandomEntropy(int size)
		{
			byte[] randomBytes = new byte[size];

			// Fill the array with cryptographically secure random bytes.
			using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
			{
				rngCsp.GetBytes(randomBytes);
			}

			return randomBytes;
		}
#endif
#if NET8_0
		private static byte[] GenerateBitsOfRandomEntropy(int size)
		{
			return RandomNumberGenerator.GetBytes(size);
		}
#endif

		private static byte[] GenerateKey(string appPrefix, string userUniqueID, byte[] salt)
		{
			using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(appPrefix + userUniqueID, salt, DERIVATION_ITERATIONS, HashAlgorithmName.SHA512))
			{
				return deriveBytes.GetBytes(KEY_SIZE / 8);
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


			// Salt and IV are randomly generated each time, and added to encrypted cipher text
			// so that the same Salt and IV values can be used when decrypting.
			byte[] salt = GenerateBitsOfRandomEntropy(BLOCK_SIZE_BYTES);
			byte[] iv = GenerateBitsOfRandomEntropy(BLOCK_SIZE_BYTES);
			byte[] key = GenerateKey(appPrefix, userUniqueID, salt);


			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = iv;

				// Create an encryptor to perform the stream transform
				ICryptoTransform encryptor = aesAlg.CreateEncryptor();

				byte[] cipherTextBytes;
				// Create the streams used for encryption
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							// Write all data to the stream
							swEncrypt.Write(plainText);
						}
					}
					cipherTextBytes = msEncrypt.ToArray();
				}

				return Convert.ToBase64String(salt.Concat(iv).Concat(cipherTextBytes).ToArray());
			}
		}

		/// <summary>Decrypts the specified data for a specific user in a specific application.</summary>
		/// <typeparam name="T">Any</typeparam>
		/// <param name="appPrefix">The application prefix.</param>
		/// <param name="userUniqueID">The user unique identifier.</param>
		/// <param name="encryptedData">The encrypted data.</param>
		/// <returns>the decrypted data</returns>
		public T Decrypt<T>(string appPrefix, string userUniqueID, string encryptedData)
		{
			if (string.IsNullOrWhiteSpace(appPrefix) || string.IsNullOrWhiteSpace(userUniqueID) || string.IsNullOrWhiteSpace(encryptedData))
			{
				throw new CryptographicException("empty appPrefix or userUniqueID or encryptedData");
			}

			// Get the complete stream of bytes that represent:
			// [BLOCK_SIZE_BYTES bytes of Salt] + [BLOCK_SIZE_BYTES bytes of IV] + [n bytes of CipherText]
			byte[] saltIvAndCipherTextBytes = Convert.FromBase64String(encryptedData);

			// Get the salt bytes by extracting the first BLOCK_SIZE_BYTES bytes from the supplied saltIvAndCipherTextBytes bytes.
			byte[] salt = saltIvAndCipherTextBytes.Take(BLOCK_SIZE_BYTES).ToArray();

			// Get the IV bytes by extracting the next BLOCK_SIZE_BYTES bytes from the supplied saltIvAndCipherTextBytes bytes.
			byte[] iv = saltIvAndCipherTextBytes.Skip(BLOCK_SIZE_BYTES).Take(BLOCK_SIZE_BYTES).ToArray();

			// Get the actual cipher text bytes by removing the first 2 * BLOCK_SIZE_BYTES bytes from the saltIvAndCipherTextBytes string.
			byte[] cipherText = saltIvAndCipherTextBytes.Skip(BLOCK_SIZE_BYTES * 2).ToArray();

			byte[] key = GenerateKey(appPrefix, userUniqueID, salt);

			string serizlizedTicket;
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = iv;

				// Create an encryptor to perform the stream transform
				ICryptoTransform decryptor = aesAlg.CreateDecryptor();

				// Create the streams used for decryption
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				using (StreamReader srDecrypt = new StreamReader(csDecrypt))
				{ 
					// Read the decrypted bytes from the decrypting stream
					serizlizedTicket = srDecrypt.ReadToEnd();
				}
			}

			Storage<T> store = JsonConvert.DeserializeObject<Storage<T>>(serizlizedTicket);

			//Ticket Decrypted successfully - but not for the right user
			if (store.AppPrefix != appPrefix || store.UserUniqueID != userUniqueID)
			{
				throw new CryptographicException($"Ticket Violation");
			}

			//Ticket Decrypted successfully - but expired
			return store.ExpirationDate.HasValue && store.ExpirationDate.Value < DateTime.UtcNow
				? throw new CryptographicException($"Data Expired {DateTime.UtcNow - store.ExpirationDate.Value} ago")
				: store.Data;
		}
	}
}
