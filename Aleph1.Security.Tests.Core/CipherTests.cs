using System.Security.Cryptography;

using Aleph1.Security.Contracts;

using Fastenshtein;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Aleph1.Security.Tests.Core
{
	[TestClass]
	public class CipherTests
	{
		private readonly ICipher cipher = new Implementation.RijndaelManagedCipher.RijndaelManagedCipher();
		private readonly string secret = "My special secret - hello world";
		private readonly string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
		private readonly string userUniqueID = "127.0.0.0";
		private readonly string wrongAppPrefix = "{4EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
		private readonly string wrongUserUniqueID = "127.1.0.0";

		[TestMethod]
		public void Decrypd_RightApp_RightClient_RightTime_Should_Work()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMinutes(1));

			Assert.AreEqual(secret, cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));
		}

		[TestMethod]
		public void Decrypd_RightApp_RightClient_UnlimitedTime_Should_Work()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

			Assert.AreEqual(secret, cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));
		}

		[TestMethod]
		public void Decrypd_RightApp_RightClient_WrongTime_Should_Fail()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMilliseconds(15));
			Thread.Sleep(30);

			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));
		}

		[TestMethod]
		public void Decrypd_RightApp_WrongClient_Should_Fail()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, wrongUserUniqueID, ticket));
		}

		[TestMethod]
		public void Decrypd_WrongtApp_Should_Fail()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(wrongAppPrefix, userUniqueID, ticket));
		}

		[TestMethod]
		public void Decrypd_NullsAndEmptyStrings()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>("", "", ticket));
			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(null, "", ticket));
			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>("", null, ticket));
			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(null, null, ticket));
			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, userUniqueID, ""));
			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, userUniqueID, null));
		}

		[TestMethod]
		public void Decrypd_ChangingOneLetter_Should_Fail()
		{
			string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

			string someChar = ticket[ticket.Length / 2] == 'a' ? "b" : "a";
			string wrongTicket = ticket.Remove(20, 1).Insert(1, someChar);

			Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, userUniqueID, wrongTicket));
		}


		[TestMethod]
		public void Encrypt_NullsAndEmptyStrings()
		{
			Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt("", "", secret));
			Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt(null, "", secret));
			Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt("", null, secret));
			Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt(null, null, secret));

			string emptyStringCipher = cipher.Encrypt(appPrefix, userUniqueID, "");
			Assert.AreEqual("", cipher.Decrypt<string>(appPrefix, userUniqueID, emptyStringCipher));

			string nullCipher = cipher.Encrypt<string>(appPrefix, userUniqueID, null);
			Assert.IsNull(cipher.Decrypt<string>(appPrefix, userUniqueID, nullCipher));
		}

		[TestMethod]
		public void Encrypt_DifferentTimes_ShouldResultInBigVariant()
		{
			string ticket1 = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMinutes(1));
			string ticket2 = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMinutes(15));
			int distance = Levenshtein.Distance(ticket1, ticket2);

			Assert.IsTrue(distance > 0.7 * ticket1.Length, "encrypted text don't vary enough");
		}
	}
}
