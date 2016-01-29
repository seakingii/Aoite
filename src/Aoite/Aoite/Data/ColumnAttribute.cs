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
    public class JsonColumnAttribute : Attribute, IPropertyPopulateAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="JsonColumnAttribute"/> 类的新实例。
        /// </summary>
        public JsonColumnAttribute() { }

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

    /// <summary>
    /// 表示一个字符串长度限制的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class StringLengthAttribute : Attribute, IPropertyValidator
    {
        /// <summary>
        /// 设置或获取一个值，指示属性检查的排序。排序越小排在越前面。
        /// </summary>
        public int Order { get; set; } = 1;
        /// <summary>
        /// 设置或获取一个值，指示字符串的最大长度，小于 1 表示不控制。
        /// </summary>
        public int MaxLength { get; }
        /// <summary>
        /// 设置或获取一个值，为 true 时 ASCII 码超过 128 的字符都会被计算 2 个字节长度，否则所有字符都只计算 1 个字节长度。
        /// </summary>
        public bool IsUnicodeCharacher { get; }

        /// <summary>
        /// 初始化一个 <see cref="StringLengthAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="maxLength">字符串的最大长度，小于 1 表示不控制。</param>
        /// <param name="isUnicodeCharacher">为 true 时 ASCII 码超过 128 的字符都会被计算 2 个字节长度，否则所有字符都只计算 1 个字节长度。</param>
        public StringLengthAttribute(int maxLength, bool isUnicodeCharacher)
        {
            this.MaxLength = maxLength;
            this.IsUnicodeCharacher = isUnicodeCharacher;
        }

        object IPropertyValidator.Validate(TypeMapper typeMapper, PropertyMapper propertyMapper, object instance, object value)
        {
            if(!(value is string) || this.MaxLength < 1) return value;

            var str = value as string;

            var length = this.IsUnicodeCharacher ? str.Length : str.GetDataLength();

            if(length > MaxLength) throw new ArgumentException($"类型{typeMapper.Type.FullName}的属性{propertyMapper.Name}值超过了限定长度 {this.MaxLength}。", propertyMapper.Name);

            return value;
        }
    }

    /// <summary>
    /// 表示一个值不允许为空的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : Attribute, IPropertyValidator
    {
        /// <summary>
        /// 设置或获取一个值，指示属性检查的排序。排序越小排在越前面。
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// 初始化一个 <see cref="NotNullAttribute"/> 类的新实例。
        /// </summary>
        public NotNullAttribute() { }

        object IPropertyValidator.Validate(TypeMapper typeMapper, PropertyMapper propertyMapper, object instance, object value)
        {
            if(value == null) throw new ArgumentNullException(propertyMapper.Name, $"类型{typeMapper.Type.FullName}的属性{propertyMapper.Name}值不能是一个空值。");
            return value;
        }
    }
}
