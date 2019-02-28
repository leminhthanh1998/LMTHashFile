using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMTHashFiles
{
    class HashFile
    {
        /// <summary>
        /// Hash file MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MD5(string path)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

        /// <summary>
        /// Hash file sha1
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string SHA1(string path)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                var stream = File.OpenRead(path);
                string hash = BitConverter
                    .ToString(cryptoProvider.ComputeHash(stream)).Replace("-", "");
                return  hash.ToLower();
            }
        }

        /// <summary>
        /// Hash file sha256
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string SHA256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
