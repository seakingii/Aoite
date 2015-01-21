using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 定义一个可自定义序列化的功能。
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// 反序列化指定字节数组。
        /// </summary>
        /// <param name="bytes">字节数组。</param>
        /// <returns>返回一个对象。</returns>
        object Deserialize(byte[] bytes);
        /// <summary>
        /// 序列化指定对象。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>返回一个字节数组。</returns>
        byte[] Serialize(object value);
    }

    /// <summary>
    /// 表示一个提供可自定义序列化的功能特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CustomAttribute : Attribute
    {
        private Type _FormatterType;
        /// <summary>
        /// 获取可自定义序列化的功能实现类型。
        /// </summary>
        public Type FormatterType { get { return _FormatterType; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.CustomAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="formatterType">可自定义序列化的功能实现类型。</param>
        public CustomAttribute(Type formatterType)
        {
            if(formatterType == null) throw new ArgumentNullException("formatterType");
            if(!Types.ISerializable.IsAssignableFrom(formatterType)) throw new ArgumentException("类型 {0} 没有实现 {1} 的接口。".Fmt(formatterType.FullName, Types.ISerializable.FullName), "type");
            if(formatterType.IsInterface || formatterType.IsAbstract) throw new ArgumentException("类型 {0} 不能是一个接口或基类。".Fmt(Types.ISerializable.FullName), "type");
            this._FormatterType = formatterType;
        }
    }
}
