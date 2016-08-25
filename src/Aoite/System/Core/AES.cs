using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 表示 AES 的加密方法。
    /// </summary>
    public static class AES
    {
        /// <summary>
        /// 使用 AES 算法对数据进行加密。
        /// </summary>
        /// <param name="data">要加密的数据。</param>
        /// <param name="key">对称算法的密钥。</param>
        /// <param name="iv">对称算法的初始化向量。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>已加密的数据。</returns>
        public static string Encrypt(string data, string key, string iv, Encoding encoding = null)
        {
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(iv == null) throw new ArgumentNullException(nameof(iv));
            if(encoding == null) encoding = GA.UTF8;

            return RSA.Base64UrlEncode(Encrypt(encoding.GetBytes(data), encoding.GetBytes(key), encoding.GetBytes(iv)));
        }

        /// <summary>
        /// 使用 AES 算法对数据进行加密。
        /// </summary>
        /// <param name="data">要加密的数据。</param>
        /// <param name="keyArray">对称算法的密钥。</param>
        /// <param name="ivArray">对称算法的初始化向量。</param>
        /// <returns>已加密的数据。</returns>
        public static byte[] Encrypt(byte[] data, byte[] keyArray, byte[] ivArray)
        {
            if(data == null) throw new ArgumentNullException(nameof(data));
            if(keyArray == null) throw new ArgumentNullException(nameof(keyArray));
            if(ivArray == null) throw new ArgumentNullException(nameof(ivArray));
            if(keyArray.Length != 16 || ivArray.Length != 16) throw new InvalidOperationException("Key 或 IV 的长度必须为 16 位。");

            var rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            return rDel.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// 使用 AES 算法对数据进行解密。
        /// </summary>
        /// <param name="data">要解密的数据。</param>
        /// <param name="key">对称算法的密钥。</param>
        /// <param name="iv">对称算法的初始化向量。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>已解密的数据。</returns>
        public static string Decrypt(string data, string key, string iv, Encoding encoding = null)
        {
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(iv == null) throw new ArgumentNullException(nameof(iv));

            if(encoding == null) encoding = GA.UTF8;
            return encoding.GetString(Decrypt(RSA.Base64UrlDecode(data), encoding.GetBytes(key), encoding.GetBytes(iv)));
        }

        /// <summary>
        /// 使用 AES 算法对数据进行解密。
        /// </summary>
        /// <param name="data">要解密的数据。</param>
        /// <param name="keyArray">对称算法的密钥。</param>
        /// <param name="ivArray">对称算法的初始化向量。</param>
        /// <returns>已解密的数据。</returns>
        public static byte[] Decrypt(byte[] data, byte[] keyArray, byte[] ivArray)
        {
            if(data == null) throw new ArgumentNullException(nameof(data));
            if(keyArray == null) throw new ArgumentNullException(nameof(keyArray));
            if(ivArray == null) throw new ArgumentNullException(nameof(ivArray));
            if(keyArray.Length != 16 || ivArray.Length != 16) throw new InvalidOperationException("Key 或 IV 的长度必须为 16 位。");

            var rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            return rDel.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }
    }
}
