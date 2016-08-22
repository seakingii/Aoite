using System.Security.Cryptography;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示数据安全处理。
    /// </summary>
    public static class DataSecurity
    {
        /// <summary>
        /// 指定哈希算法，哈希指定的文本。
        /// </summary>
        /// <param name="alog">哈希算法。</param>
        /// <param name="text">需哈希的字符串。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>哈希后的字符串。</returns>
        public static string Crypto(HashAlgorithms alog, string text, Encoding encoding = null)
        {
            return Crypto(alog, (encoding ?? GA.UTF8).GetBytes(text)).ToHexString();
        }


        /// <summary>
        /// 指定哈希算法，哈希指定的。
        /// </summary>
        /// <param name="alog">哈希算法。</param>
        /// <param name="bytes">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        public static byte[] Crypto(HashAlgorithms alog, byte[] bytes)
        {
            HashAlgorithm algorithm;
            switch(alog)
            {
                case HashAlgorithms.SHA1:
                    algorithm = CreateSHA1();
                    break;
                case HashAlgorithms.SHA256:
                    algorithm = CreateSHA256();
                    break;
                case HashAlgorithms.SHA384:
                    algorithm = CreateSHA384();
                    break;
                case HashAlgorithms.SHA512:
                    algorithm = CreateSHA512();
                    break;
                case HashAlgorithms.MD5:
                    algorithm = CreateMD5();
                    break;
                default: throw new NotSupportedException();
            }
            using(algorithm)
            {
                return algorithm.ComputeHash(bytes);
            }
        }

        /// <summary>
        /// 生产成指定字符串，生成 32位加盐值，并返回 44 位加盐散列后的文本。
        /// </summary>
        /// <param name="text">原始文本。</param>
        /// <param name="salt">加盐值。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns> 44 位加盐散列后的文本。</returns>
        public static string GenerateSaltedHash(string text, out Guid salt, Encoding encoding = null) => GenerateSaltedHash(text, salt = Guid.NewGuid(), encoding);

        /// <summary>
        /// 生产成指定字符串和加盐值，并返回 44 位加盐散列后的文本。
        /// </summary>
        /// <param name="text">原始文本。</param>
        /// <param name="salt">加盐值。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns> 44 位加盐散列后的文本。</returns>
        public static string GenerateSaltedHash(string text, Guid salt, Encoding encoding = null)
        {
            if(encoding == null) encoding = GA.UTF8;
            return Convert.ToBase64String(GenerateSaltedHash(encoding.GetBytes(text), encoding.GetBytes(salt.ToString("N"))));
        }

        static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for(int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for(int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        internal static SHA1 CreateSHA1() => SHA1.Create();

        internal static SHA256 CreateSHA256() => SHA256.Create();

        internal static SHA384 CreateSHA384() => SHA384.Create();

        internal static SHA512 CreateSHA512() => SHA512.Create();

        internal static MD5 CreateMD5() => MD5.Create();

    }


    /// <summary>
    /// 表示安全哈希算法。
    /// </summary>
    public enum HashAlgorithms
    {
        /// <summary>
        /// 使用 <see cref="Security.Cryptography.SHA1"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA1,
        /// <summary>
        /// 使用 <see cref="Security.Cryptography.SHA256"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA256,
        /// <summary>
        /// 使用 <see cref="Security.Cryptography.SHA384"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA384,
        /// <summary>
        /// 使用 <see cref="Security.Cryptography.SHA512"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA512,
        /// <summary>
        ///  提供 MD5（消息摘要 5）128 位哈希算法的实现。
        /// </summary>
        MD5
    }
}
