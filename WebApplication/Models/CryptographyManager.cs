using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Models
{
    /// <summary>
    /// http://www.codeproject.com/Articles/38796/Simple-Encrypt-and-Decrypt-Technique-and-Computing
    /// </summary>
    public class CryptographyManager
    {
        //Default initial vector
        private readonly byte[] _ivByte = {0x01, 0x12, 0x23, 0x34, 0x45, 0x56, 0x67, 0x78};
        private byte[] _keyByte = {};
        
        /// <summary> 
        /// Encrypt text by key with initialization vector 
        /// </summary> 
        /// <param name="value">plain text</param> 
        /// <param name="key"> string key</param> 
        /// <returns>encrypted text</returns> 
        public string Encrypt(string value, string key)
        {
            string encryptValue = string.Empty;
            MemoryStream ms = null;
            CryptoStream cs = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    _keyByte = Encoding.UTF8.GetBytes
                        (key.Substring(0, 8));


                    using (var des =
                        new DESCryptoServiceProvider())
                    {
                        byte[] inputByteArray =
                            Encoding.UTF8.GetBytes(value);
                        ms = new MemoryStream();
                        cs = new CryptoStream(ms, des.CreateEncryptor
                                                      (_keyByte, _ivByte), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        encryptValue = Convert.ToBase64String(ms.ToArray());
                    }
                }
                finally
                {
                    if (cs != null) cs.Dispose();
                    if (ms != null) ms.Dispose();
                }
            }
            return encryptValue;
        }

        /// <summary> 
        /// Decrypt text by key with initialization vector 
        /// </summary> 
        /// <param name="value">encrypted text</param> 
        /// <param name="key"> string key</param> 
        /// <returns>encrypted text</returns> 
        public string Decrypt(string value, string key)
        {
            string decrptValue = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                MemoryStream ms = null;
                CryptoStream cs = null;
                value = value.Replace(" ", "+");
                try
                {
                    _keyByte = Encoding.UTF8.GetBytes
                        (key.Substring(0, 8));

                    using (var des =
                        new DESCryptoServiceProvider())
                    {
                        byte[] inputByteArray = Convert.FromBase64String(value);
                        ms = new MemoryStream();
                        cs = new CryptoStream(ms, des.CreateDecryptor
                                                      (_keyByte, _ivByte), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        Encoding encoding = Encoding.UTF8;
                        decrptValue = encoding.GetString(ms.ToArray());
                    }
                }
                finally
                {
                    if (cs != null) cs.Dispose();
                    if (ms != null) ms.Dispose();
                }
            }
            return decrptValue;
        }

        /// <summary> 
        /// Compute Hash 
        /// </summary> 
        /// <param name="plainText">plain text</param> 
        /// <param name="salt">salt string</param> 
        /// <returns>string</returns> 
        public string ComputeHash(string plainText, string salt)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                // Convert plain text into a byte array. 
                byte[] plainTextBytes = Encoding.ASCII.GetBytes(plainText);
                // Allocate array, which will hold plain text and salt. 
                byte[] plainTextWithSaltBytes = null;
                byte[] saltBytes;
                if (!string.IsNullOrEmpty(salt))
                {
                    // Convert salt text into a byte array. 
                    saltBytes = Encoding.ASCII.GetBytes(salt);
                    plainTextWithSaltBytes =
                        new byte[plainTextBytes.Length + saltBytes.Length];
                }
                else
                {
                    // Define min and max salt sizes. 
                    const int minSaltSize = 4;
                    const int maxSaltSize = 8;
                    // Generate a random number for the size of the salt. 
                    var random = new Random();
                    int saltSize = random.Next(minSaltSize, maxSaltSize);
                    // Allocate a byte array, which will hold the salt. 
                    saltBytes = new byte[saltSize];
                    // Initialize a random number generator. 
                    var rngCryptoServiceProvider =
                        new RNGCryptoServiceProvider();
                    // Fill the salt with cryptographically strong byte values. 
                    rngCryptoServiceProvider.GetNonZeroBytes(saltBytes);
                }
                // Copy plain text bytes into resulting array. 
                for (int i = 0; i < plainTextBytes.Length; i++)
                {
                    if (plainTextWithSaltBytes != null) plainTextWithSaltBytes[i] = plainTextBytes[i];
                }
                // Append salt bytes to the resulting array. 
                for (int i = 0; i < saltBytes.Length; i++)
                {
                    if (plainTextWithSaltBytes != null)
                        plainTextWithSaltBytes[plainTextBytes.Length + i] =
                            saltBytes[i];
                }
                HashAlgorithm hash = new SHA1Managed();
                // Compute hash value of our plain text with appended salt. 
                if (plainTextWithSaltBytes != null)
                {
                    byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
                    // Create array which will hold hash and original salt bytes. 
                    var hashWithSaltBytes =
                        new byte[hashBytes.Length + saltBytes.Length];
                    // Copy hash bytes into resulting array. 
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        hashWithSaltBytes[i] = hashBytes[i];
                    }
                    // Append salt bytes to the result. 
                    for (int i = 0; i < saltBytes.Length; i++)
                    {
                        hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];
                    }
                    // Convert result into a base64-encoded string. 
                    string hashValue = Convert.ToBase64String(hashWithSaltBytes);
                    // Return the result. 
                    return hashValue;
                }
            }
            return string.Empty;
        }
    }
}