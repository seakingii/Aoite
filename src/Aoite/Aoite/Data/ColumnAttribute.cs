using System;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个具有列的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : AliasAttribute, IKeyAttribute
    {
        /// <summary>
        /// 指示当前属性是否为主要成员，初始化一个 <see cref="ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="isPrimaryKey">指示当前属性是否为主要成员。</param>
        public ColumnAttribute(bool isPrimaryKey) : this(null, isPrimaryKey) { }

        /// <summary>
        /// 指定名称，初始化一个 <see cref="ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        public ColumnAttribute(string name) : this(name, false) { }

        /// <summary>
        /// 指定名称和指示当前属性是否为主要成员，初始化一个 <see cref="ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="isKey">指示当前属性是否为主要成员。</param>
        public ColumnAttribute(string name, bool isKey)
            : base(name)
        {
            this.IsKey = isKey;
        }

        /// <summary>
        /// 初始化一个空的 <see cref="ColumnAttribute"/> 类的新实例。
        /// </summary>
        public ColumnAttribute() { }

        /// <summary>
        /// 获取或设置一个值，该值指示当前属性是否为主键。
        /// </summary>
        public bool IsKey { get; set; }
    }

    /// <summary>
    /// 表示一个具有列的特性，并且这个列对应的属性应可序列化/反序列化为一个 JSON。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class JsonColumnAttribute : AliasAttribute, IPropertyPopulateAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="JsonColumnAttribute"/> 类的新实例。
        /// </summary>
        public JsonColumnAttribute() { }

        /// <summary>
        /// 指定名称，初始化一个 <see cref="JsonColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        public JsonColumnAttribute(string name) : base(name) { }

        /// <summary>
        /// 指定一个实例，获取指定属性的值。
        /// </summary>
        /// <param name="property">动态属性。</param>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <returns>属性的值。</returns>
        public object GetValue(DynamicProperty property, object instance)
        {
            var s = property.GetValue(instance, false);

            return ObjectFactory.Context.Get<IJsonProvider>().Serialize(s);
        }

        /// <summary>
        /// 指定一个实例，设置指定属性的值。
        /// </summary>
        /// <param name="property">动态属性。</param>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        public void SetValue(DynamicProperty property, object instance, object value)
        {
            if(value is string)
            {
                value = ObjectFactory.Context.Get<IJsonProvider>().Deserialize(value.ToString(), property.Property.PropertyType);
            }
            property.SetValue(instance, value, false);
        }
    }
}
