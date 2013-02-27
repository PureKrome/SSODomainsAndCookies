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
            //var x = new AesCryptoServiceProvider();
            //x.KeySize = 128;
            //x.GenerateIV();
            //x.GenerateKey();

            //var genB = x.Key;
            //var genIV = x.IV;

            //var k = Convert.ToBase64String(x.Key);
            //var v = Convert.ToBase64String(x.IV);

            key = "ZcOLAbbz13RVnQ7uPJKH8w==";
            iv = "WuH5s2BF/UENerWnrELgfA==";

            var bKey = Convert.FromBase64String(key);
            var bIV = Convert.FromBase64String(iv);


            SymmetricAlgorithm algorithm = new T();
            algorithm.KeySize = 128;

            ICryptoTransform transform = algorithm.CreateEncryptor(bKey, bIV);

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
            key = "ZcOLAbbz13RVnQ7uPJKH8w==";
            iv = "WuH5s2BF/UENerWnrELgfA==";

            var bKey = Convert.FromBase64String(key);
            var bIV = Convert.FromBase64String(iv);

            SymmetricAlgorithm algorithm = new T();
            algorithm.KeySize = 128;

            ICryptoTransform transform = algorithm.CreateEncryptor(bKey, bIV);

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