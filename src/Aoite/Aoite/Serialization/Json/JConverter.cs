using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Serialization.Json
{
    /// <summary>
    /// 为自定义类型转换器提供抽象基类。
    /// </summary>
    public abstract class JConverter
    {
        /// <summary>
        /// 初始化 <see cref="JConverter"/> 类的新实例。
        /// </summary>
        protected JConverter() { }
        /// <summary>
        /// 当在派生类中重写时，判定给定的类型是否支持。
        /// </summary>
        /// <param name="type">判断的类型。</param>
        /// <returns>支持返回 true，否则返回 false。</returns>
        public abstract bool IsSupported(Type type);

        /// <summary>
        /// 当在派生类中重写时，将所提供的字典转换为指定类型的对象。
        /// </summary>
        /// <param name="dictionary">作为名称/值对存储的属性数据的 <see cref="IDictionary{TKey,TValue}"/> 实例。</param>
        /// <param name="type">所生成对象的类型。</param>
        /// <param name="serializer"><see cref="JSerializer"/> 实例。</param>
        /// <returns>反序列化的对象。</returns>
        public abstract object Deserialize(IDictionary<string, object> dictionary, Type type, JSerializer serializer);

        /// <summary>
        /// 当在派生类中重写时，生成名称/值对的字典。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="serializer">负责序列化的对象。</param>
        /// <returns>包含表示该对象数据的键/值对。</returns>
        public abstract IDictionary<string, object> Serialize(object obj, JSerializer serializer);
    }
}
