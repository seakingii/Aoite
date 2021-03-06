﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 表示 RSA 的加密方法。
    /// </summary>
    public static class RSA
    {
        const int DefaultKeySize = 1024;

        internal static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        internal static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch(output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break;  // One pad char
                default: throw new InvalidCastException();
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        /// <summary>
        /// 如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 <see cref="Security.Cryptography.RSA"/> 解密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。
        /// </summary>
        public static bool RSA_FOAEP = true;
        /// <summary>
        /// 创建一个 RSA 的密钥信息。
        /// </summary>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        /// <returns>一个 RSA 的密钥信息。</returns>
        public static RSAKey Create(int dwKeySize = DefaultKeySize) => new RSAKey(dwKeySize);

        /// <summary>
        /// 使用 <see cref="Security.Cryptography.RSA"/> 算法对数据进行加密。
        /// </summary>
        /// <param name="data">要加密的数据。</param>
        /// <param name="publicKey">RSA 公钥。</param>
        /// <param name="encoding">编码方式。</param>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        /// <returns>已加密的数据。</returns>
        public static string Encrypt(string data, string publicKey, Encoding encoding = null, int dwKeySize = DefaultKeySize)
        {
            if(encoding == null) encoding = GA.UTF8;

            return Base64UrlEncode(Encrypt(encoding.GetBytes(data), publicKey, dwKeySize));
        }

        /// <summary>
        /// 使用 <see cref="Security.Cryptography.RSA"/> 算法对数据进行加密。
        /// </summary>
        /// <param name="data">要加密的数据。</param>
        /// <param name="publicKey">RSA 公钥。</param>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        /// <returns>已加密的数据。</returns>
        public static byte[] Encrypt(byte[] data, string publicKey, int dwKeySize = DefaultKeySize)
        {
            using(var rsaProvider = new RSACryptoServiceProvider(dwKeySize))
            {
                return Encrypt(rsaProvider, data, publicKey);
            }
        }

        /// <summary>
        /// 使用 <see cref="Security.Cryptography.RSA"/> 算法对数据进行加密。
        /// </summary>
        /// <param name="rsaProvider"><see cref="RSACryptoServiceProvider"/> 的实例。</param>
        /// <param name="data">要加密的数据。</param>
        /// <param name="publicKey">RSA 公钥。</param>
        /// <returns>已加密的数据。</returns>
        public static byte[] Encrypt(RSACryptoServiceProvider rsaProvider, byte[] data, string publicKey)
        {
            if(rsaProvider == null) throw new ArgumentNullException(nameof(rsaProvider));
            rsaProvider.FromXmlString(publicKey);
            return rsaProvider.Encrypt(data, RSA_FOAEP);
        }

        /// <summary>
        /// 使用 <see cref="Security.Cryptography.RSA"/> 算法对数据进行解密。
        /// </summary>
        /// <param name="data">要解密的数据。</param>
        /// <param name="key">包含 RSA 公钥和私钥。</param>
        /// <param name="encoding">编码方式。</param>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        /// <returns>已解密的数据，它是加密前的原始纯文本。</returns>
        public static string Decrypt(string data, string key, Encoding encoding = null, int dwKeySize = DefaultKeySize)
        {
            if(encoding == null) encoding = GA.UTF8;

            return encoding.GetString(Decrypt(Base64UrlDecode(data), key, dwKeySize));
        }

        /// <summary>
        /// 使用 <see cref="Security.Cryptography.RSA"/> 算法对数据进行解密。
        /// </summary>
        /// <param name="data">要解密的数据。</param>
        /// <param name="key">包含 RSA 公钥和私钥。</param>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        /// <returns>已解密的数据，它是加密前的原始纯文本。</returns>
        public static byte[] Decrypt(byte[] data, string key, int dwKeySize = DefaultKeySize)
        {
            using(var rsaProvider = new RSACryptoServiceProvider(dwKeySize))
            {
                return Decrypt(rsaProvider, data, key);
            }
        }

        private static byte[] Decrypt(RSACryptoServiceProvider rsaProvider, byte[] data, string key)
        {
            if(rsaProvider == null) throw new ArgumentNullException(nameof(rsaProvider));
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            rsaProvider.FromXmlString(key);
            return rsaProvider.Decrypt(data, RSA_FOAEP);
        }
    }

    /// <summary>
    /// 表示一个 RSA 的密钥信息。
    /// </summary>
    public class RSAKey
    {
        /// <summary>
        /// 获取一个值，表示 RSA 公钥。
        /// </summary>
        public string PublicKey { get; }
        /// <summary>
        /// 获取一个值，表示同时包含 RSA 公钥和私钥。
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 获取当前密钥的大小。
        /// </summary>
        public int KeySize { get; }

        /// <summary>
        /// 使用指定的密钥大小初始化 <see cref="RSAKey"/> 类的新实例。
        /// </summary>
        /// <param name="dwKeySize">要使用的密钥的大小（以位为单位）。</param>
        public RSAKey(int dwKeySize) : this(new RSACryptoServiceProvider(dwKeySize)) { }

        /// <summary>
        /// 使用 <see cref="RSACryptoServiceProvider"/> 实例，初始化 <see cref="RSAKey"/> 类的新实例。
        /// </summary>
        /// <param name="rsaProvider"><see cref="RSACryptoServiceProvider"/> 的实例。</param>
        public RSAKey(RSACryptoServiceProvider rsaProvider)
        {
            if(rsaProvider == null) throw new ArgumentNullException(nameof(rsaProvider));
            PublicKey = rsaProvider.ToXmlString(false);
            Key = rsaProvider.ToXmlString(true);
            KeySize = rsaProvider.KeySize;
        }
    }



    //public class CryptoData
    //{
    //    public Encoding Encoding { get; set; }

    //    public CryptoData() { }
    //}
}
