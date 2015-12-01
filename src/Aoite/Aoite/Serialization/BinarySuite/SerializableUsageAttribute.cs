using System;

namespace Aoite.Serialization
{

    /// <summary>
    /// 表示一个提供可自定义序列化的功能特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializableUsageAttribute : Attribute
    {
        private Type _FormatterType;
        /// <summary>
        /// 获取可自定义序列化的功能实现类型。
        /// </summary>
        public Type FormatterType { get { return _FormatterType; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.SerializableUsageAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="formatterType">可自定义序列化的功能实现类型。</param>
        public SerializableUsageAttribute(Type formatterType)
        {
            if(formatterType == null) throw new ArgumentNullException(nameof(formatterType));
            if(!Types.ISerializable.IsAssignableFrom(formatterType)) throw new ArgumentException("类型 {0} 没有实现 {1} 的接口。".Fmt(formatterType.FullName, Types.ISerializable.FullName), "type");
            if(formatterType.IsInterface || formatterType.IsAbstract) throw new ArgumentException("类型 {0} 不能是一个接口或基类。".Fmt(Types.ISerializable.FullName), "type");
            this._FormatterType = formatterType;
        }
    }
}
