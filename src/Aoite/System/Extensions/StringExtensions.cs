using System.Globalization;
using System.Text;
using System.Linq;

namespace System
{
    /// <summary>
    /// 提供用于字符串值的实用工具方法。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 返回当前字符串的 MD5 哈希后小写形式的字符串。
        /// </summary>
        /// <param name="text">需哈希的字符串。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>哈希后的字符串。</returns>
        public static string ToMd5(this string text, Encoding encoding = null)
            => DataSecurity.Crypto(HashAlgorithms.MD5, text, encoding).ToLower();

        /// <summary>
        /// 返回当前字节数组的 MD5 哈希后小写形式的字符串。
        /// </summary>
        /// <param name="bytes">需哈希的字节数组。</param>
        /// <returns>哈希后的字符串。</returns>
        public static string ToMd5(this byte[] bytes)
            => DataSecurity.Crypto(HashAlgorithms.MD5, bytes).ToHexString().ToLower();
        /// <summary>
        /// 将当前字符串转换为智能小写模式。
        /// </summary>
        /// <param name="s">当前字符串。</param>
        /// <returns>新的字符串。</returns>
        public static string ToCamelCase(this string s)
        {
            if(s == null || s.Length == 0) return s;

            if(!char.IsUpper(s[0])) return s;

            //var c = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString();
            //if(s.Length > 1) return c + s.Substring(1);
            //return c;

            var chars = s.ToCharArray();

            for(int i = 0; i < chars.Length; i++)
            {
                var hasNext = (i + 1 < chars.Length);
                if(i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }

        /// <summary>
        /// 将当前字符串转换为指定编码的的字节组。
        /// </summary>
        /// <param name="value">当前字符串。</param>
        /// <param name="encoding">编码。为 null 值表示 UTF8 的编码。</param>
        /// <returns>字节组。</returns>
        public static byte[] ToBytes(this string value, Encoding encoding = null)
            => (encoding ?? GA.UTF8).GetBytes(value);

        /// <summary>
        /// 忽略被比较字符串的大小写，确定两个指定的 <see cref="String"/> 实例是否具有同一值。
        /// </summary>
        /// <param name="a"><see cref="String"/>第一个 <see cref="String"/> 的实例。</param>
        /// <param name="b"><see cref="String"/>第二个 <see cref="String"/> 的实例。</param>
        /// <returns>如果 <paramref name="a"/> 参数的值等于 <paramref name="b"/> 参数的值，则为 true；否则为 false。</returns>
        public static bool iEquals(this string a, string b)
            => string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// 忽略被比较字符串的大小写，确定在使用指定的比较选项进行比较时此字符串实例的开头是否与指定的字符串匹配。
        /// </summary>
        /// <param name="a"><see cref="String"/>第一个 <see cref="String"/> 的实例。</param>
        /// <param name="b"><see cref="String"/>第二个 <see cref="String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数与此字符串的开头匹配，则为 true；否则为 false。 </returns>
        public static bool iStartsWith(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.StartsWith(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，确定使用指定的比较选项进行比较时此字符串实例的结尾是否与指定的字符串匹配。
        /// </summary>
        /// <param name="a"><see cref="String"/>第一个 <see cref="String"/> 的实例。</param>
        /// <param name="b"><see cref="String"/>第二个 <see cref="String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数与此字符串的结尾匹配，则为 true；否则为 false。 </returns>
        public static bool iEndsWith(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.EndsWith(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，返回一个值，该值指示指定的 <see cref="String"/> 对象是否出现在此字符串中。
        /// </summary>
        /// <param name="a"><see cref="String"/>第一个 <see cref="String"/> 的实例。</param>
        /// <param name="b"><see cref="String"/>第二个 <see cref="String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数出现在此字符串中，或者 <paramref name="b"/> 为空字符串 ("")，则为 true；否则为 false。 </returns>
        public static bool iContains(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.ToLower().Contains(b.ToLower());
        }

        /// <summary>
        /// 在当前字符串的前后增加“%”符号。
        /// </summary>
        /// <param name="input">当前字符串。</param>
        /// <returns>新的字符串。</returns>
        public static string ToLiking(this string input)
            => string.Concat("%", input, "%");

        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns><paramref name="format"/> 的副本，其中的格式项已替换为 <paramref name="args"/> 中相应对象的字符串表示形式。</returns>
        public static string Fmt(this string format, params object[] args)
            => string.Format(format, args);

        /// <summary>
        /// 返回表示当前 <see cref="String"/>，如果 <paramref name="input"/> 是一个 null 值，将返回 <see cref="String.Empty"/>。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <returns> <paramref name="input"/> 的 <see cref="String"/> 或 <see cref="String.Empty"/>。</returns>
        public static string ToStringOrEmpty(this string input)
            => input ?? string.Empty;

        /// <summary>
        /// 判定当前字符串是否是一个空的字符串。
        /// </summary>
        /// <param name="input">当前字符串。</param>
        /// <returns>如果字符串为 null、空 或 空白，将返回 true，否则返回 false。</returns>
        public static bool IsNull(this string input)
            => string.IsNullOrWhiteSpace(input);

        /// <summary>
        /// 将指定的字节数组转换成十六进制的字符串。
        /// </summary>
        /// <param name="source">一个字节数组。</param>
        /// <returns>由字节数组转换后的十六进制的字符串。</returns>
        public static string ToHexString(this byte[] source)
            => BitConverter.ToString(source).Replace("-", string.Empty);

        /// <summary>
        /// 指定整串字符串的最大长度，剪裁字符串数据，超出部分将会在结尾添加“...”。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="maxLength">字符串的最大长度（含）。</param>
        /// <param name="ellipsis">指定省略号的字符串，默认为“...”。</param>
        /// <returns>新的字符串 -或- 原字符串，该字符串的最大长度不超过 <paramref name="maxLength"/>。</returns>
        public static string CutString(this string input, int maxLength, string ellipsis = "...")
        {
            if(input == null || input.Length <= maxLength) return input;
            if(string.IsNullOrWhiteSpace(ellipsis)) throw new ArgumentNullException(nameof(ellipsis));
            maxLength = maxLength - ellipsis.Length;
            return input.Substring(0, maxLength) + ellipsis;
        }

        /// <summary>
        /// 截取字符串开头的内容。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="length">获取的字符串长度。</param>
        /// <returns>新的字符串。</returns>
        public static string Starts(this string input, int length)
        {
            if(string.IsNullOrWhiteSpace(input)) return string.Empty;
            return length >= input.Length ? input : input.Substring(0, length);
        }

        /// <summary>
        /// 截取字符串结尾的内容。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="length">获取的字符串长度。</param>
        /// <returns>新的字符串。</returns>
        public static string Ends(this string input, int length)
        {
            if(string.IsNullOrWhiteSpace(input)) return string.Empty;
            return length >= input.Length ? input : input.Substring(input.Length - length);
        }

        /// <summary>
        /// 删除当前字符串的开头的字符串。
        /// </summary>
        /// <param name="val">目标字符串。</param>
        /// <param name="count">要删除的字长度。</param>
        /// <returns>删除后的字符串。</returns>
        public static string RemoveStarts(this string val, int count = 1)
        {
            if(string.IsNullOrWhiteSpace(val) || val.Length < count) return val;
            return val.Remove(0, count);
        }

        /// <summary>
        /// 删除当前字符串的结尾的字符串。
        /// </summary>
        /// <param name="val">目标字符串。</param>
        /// <param name="count">要删除的字长度。</param>
        /// <returns>删除后的字符串。</returns>
        public static string RemoveEnds(this string val, int count = 1)
        {
            if(string.IsNullOrWhiteSpace(val) || val.Length < count) return val;
            return val.Remove(val.Length - count);
        }

        /// <summary>
        /// 获取字符串的字节数。
        /// </summary>
        /// <param name="val">目标字符串。</param>
        /// <returns>字符串的字节数。</returns>
        public static int GetDataLength(this string val)
        {
            if(val == null || val.Length == 0) return 0;

            CharEnumerator ce = val.GetEnumerator();
            int length = 0;
            while(ce.MoveNext())
                length += (ce.Current >= 0 && ce.Current <= 128) ? 1 : 2;
            return length;
            //return Encoding.Default.GetByteCount(val);
        }
    }
}
