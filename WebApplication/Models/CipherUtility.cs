using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Models
{
    public class CipherUtility
    {
        public static string Encrypt<T>(string value, string key, string iv)
            where T : SymmetricAlgorithm, new()
        {
            SymmetricAlgorithm algorithm = new T();

            ICryptoTransform transform = algorithm.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(iv));

            using (var buffer = new MemoryStream())
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string Decrypt<T>(string text, string key, string iv)
            where T : SymmetricAlgorithm, new()
        {
            SymmetricAlgorithm algorithm = new T();

            ICryptoTransform transform = algorithm.CreateDecryptor(Convert.FromBase64String(key), Convert.FromBase64String(iv));

            using (var buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(stream, true))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}