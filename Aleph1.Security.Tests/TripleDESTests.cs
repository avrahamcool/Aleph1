﻿using Aleph1.Security.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Aleph1.Security.Tests
{
    [TestClass]
    public class TripleDESTests
    {
        private readonly ICipher cipher = new Implementation._3DES.TripleDES();
        private readonly string secret = "My special secret - hello world";

        [TestMethod]
        public void Decryped_RightApp_RightClient_RightTime_Should_Work()
        {
            string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string userUniqueID = "127.0.0.0";
            string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMinutes(1));

            Assert.AreEqual(secret, cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));
        }

        [TestMethod]
        public void Decryped_RightApp_RightClient_UnlimitedTime_Should_Work()
        {
            string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string userUniqueID = "127.0.0.0";
            string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

            Assert.AreEqual(secret, cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));
        }

        [TestMethod]
        public void Decryped_RightApp_RightClient_WrongTime_Should_Fail()
        {
            string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string userUniqueID = "127.0.0.0";
            string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret, TimeSpan.FromMilliseconds(15));
            Thread.Sleep(30);

            Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, userUniqueID, ticket));

        }

        [TestMethod]
        public void Decryped_RightApp_WrongClient_Should_Fail()
        {
            string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string userUniqueID = "127.0.0.0";
            string wrongUserUniqueID = "127.1.0.0";
            string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

            Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(appPrefix, wrongUserUniqueID, ticket));
        }

        [TestMethod]
        public void Decryped_WrongtApp_Should_Fail()
        {
            string appPrefix = "{3EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string wrongAppPrefix = "{4EE06365-D5E3-4D2E-A8D0-1F6E10138D29}";
            string userUniqueID = "127.0.0.0";
            string ticket = cipher.Encrypt(appPrefix, userUniqueID, secret);

            Assert.ThrowsException<CryptographicException>(() => cipher.Decrypt<string>(wrongAppPrefix, userUniqueID, ticket));
        }

        [TestMethod]
        public void Decryped_NullsAndEmptyStrings()
        {
            Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt("", "", secret));
            Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt(null, "", secret));
            Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt("", null, secret));
            Assert.ThrowsException<CryptographicException>(() => cipher.Encrypt(null, null, secret));
        }
    }
}
