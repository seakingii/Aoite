using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示一个属性的映射器。
    /// </summary>
    public class PropertyMapper : DynamicProperty
    {
        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        public virtual string Name { get; }
        /// <summary>
        /// 获取一个值，指示是否为唯一标识。
        /// </summary>
        public virtual bool IsKey { get; }

        /// <summary>
        /// 获取属性所属的类型映射器。
        /// </summary>
        public virtual TypeMapper TypeMapper { get; }

        /// <summary>
        /// 获取或设置一个值，该值指示当前成员是否已标识忽略标识。
        /// </summary>
        public virtual bool IsIgnore { get; }

        private Lazy<object> _LazyTypeDefaultValue;
        /// <summary>
        /// 获取类型的默认值。
        /// </summary>
        public object TypeDefaultValue { get { return this._LazyTypeDefaultValue.Value; } }

        /// <summary>
        /// 获取属性验证器数组。
        /// </summary>
        public IPropertyValidator[] Validators { get; }

        /// <summary>
        /// 指定属性元数据，初始化一个 <see cref="PropertyMapper"/> 类的新实例。
        /// </summary>
        /// <param name="typeMapper">类型的映射器。</param>
        /// <param name="property">成员的属性元数据。</param>
        public PropertyMapper(TypeMapper typeMapper, PropertyInfo property)
            : base(property)
        {
            if(typeMapper == null) throw new ArgumentNullException(nameof(typeMapper));
            this.TypeMapper = typeMapper;
            this.IsIgnore = property.GetAttribute<IgnoreAttribute>() != null;
            this._LazyTypeDefaultValue = new Lazy<object>(property.PropertyType.GetDefaultValue);

            var aliasAttr = property.GetAttribute<IAliasAttribute>();
            this.Name = aliasAttr != null && aliasAttr.Name != null
                ? aliasAttr.Name
                : property.Name;

            var keyAttr = property.GetAttribute<IKeyAttribute>();
            this.IsKey = (keyAttr != null && keyAttr.IsKey) || string.Equals(property.Name, DbExtensions.DefaultKeyName, StringComparison.CurrentCultureIgnoreCase);

            this.Validators = property.GetAttributes<IPropertyValidator>().ToArray();
        }

        /// <summary>
        /// 检验指定实例的属性值。
        /// </summary>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        /// <returns>返回属性值。</returns>
        protected override object Validate(object instance, object value)
        {
            foreach(var validator in this.Validators)
            {
                value = validator.Validate(this.TypeMapper, this, instance, value);
            }
            return base.Validate(instance, value);
        }
    }

    /// <summary>
    /// 定义一个属性的验证器。
    /// </summary>
    public interface IPropertyValidator
    {
        /// <summary>
        /// 获取或设置一个值，指示属性检查的排序。排序越小排在越前面。
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 验证指定属性的值。
        /// </summary>
        /// <param name="typeMapper">类型的映射器。</param>
        /// <param name="propertyMapper">属性的映射器。</param>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        /// <returns>返回新的属性值。</returns>
        object Validate(TypeMapper typeMapper, PropertyMapper propertyMapper, object instance, object value);
    }
}
