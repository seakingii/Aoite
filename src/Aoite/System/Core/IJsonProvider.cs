using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 定义一个 JSON 序列化的提供程序。
    /// </summary>
    [DefaultMapping(typeof(DefaultJsonProvider))]
    public interface IJsonProvider
    {
        /// <summary>
        /// 将 JSON 格式字符串转换为指定类型的对象。
        /// </summary>
        /// <param name="input">要反序列化的 JSON 字符串。</param>
        /// <param name="targetType">所生成对象的类型。</param>
        /// <returns>反序列化的对象。</returns>
        object Deserialize(string input, Type targetType);
        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        string Serialize(object obj);
    }

    [SingletonMapping]
    class DefaultJsonProvider : IJsonProvider
    {
        public object Deserialize(string input, Type targetType)
        {
            return Serializer.Json.Native.Deserialize(input, targetType);
        }

        public string Serialize(object obj)
        {
            return Serializer.Json.Native.Serialize(obj);
        }
    }
}
