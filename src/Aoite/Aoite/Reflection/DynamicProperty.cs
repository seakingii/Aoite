using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示一个动态的属性。
    /// </summary>
    public class DynamicProperty
    {
        /// <summary>
        /// 获取成员的属性元数据。
        /// </summary>
        public virtual PropertyInfo Property { get; }

        private Lazy<DynamicMemberSetter> lazy_setValue;
        /// <summary>
        /// 获取属性的设置器。
        /// </summary>
        public virtual DynamicMemberSetter SetValueHandler { get { return this.lazy_setValue.Value; } }

        private Lazy<DynamicMemberGetter> lazy_getValue;
        /// <summary>
        /// 获取属性的读取器。
        /// </summary>
        public virtual DynamicMemberGetter GetValueHandler { get { return this.lazy_getValue.Value; } }

        /// <summary>
        /// 获取动态属性移植的特性。
        /// </summary>
        public IPropertyPopulateAttribute Populate { get; }

        /// <summary>
        /// 提供成员的属性元数据，初始化一个 <see cref="DynamicProperty"/> 类的新实例
        /// </summary>
        /// <param name="propertyInfo">成员的属性元数据。</param>
        public DynamicProperty(PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));
            this.Property = propertyInfo;
            this.lazy_setValue = new Lazy<DynamicMemberSetter>(this.Property.CreatePropertySetter);
            this.lazy_getValue = new Lazy<DynamicMemberGetter>(this.Property.CreatePropertyGetter);
            this.Populate = propertyInfo.GetAttribute<IPropertyPopulateAttribute>();
        }

        /// <summary>
        /// 检验指定实例的属性值。
        /// </summary>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        /// <returns>返回属性值。</returns>
        protected virtual object Validate(object instance, object value) { return value; }

        /// <summary>
        /// 指定一个实例，设置当前属性的值。
        /// </summary>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        /// <param name="allowExtend">指示是否允许扩展的特性。</param>
        public virtual void SetValue(object instance, object value, bool allowExtend = false)
        {
            if(allowExtend)
            {
                var populate = Populate;
                if(populate != null)
                {
                    Populate.SetValue(this, instance, this.Validate(instance, value));
                    return;
                }
                value = this.Validate(instance, value);
            }

            this.SetValueHandler(instance, value);
        }

        /// <summary>
        /// 指定一个实例，获取当前属性的值。
        /// </summary>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="allowExtend">指示是否允许扩展的特性。</param>
        /// <returns>属性的值。</returns>
        public virtual object GetValue(object instance, bool allowExtend = false)
        {
            if(allowExtend)
            {
                var populate = Populate;
                if(populate != null)
                {
                    return this.Validate(instance, Populate.GetValue(this, instance));
                }
                return this.Validate(instance, this.GetValueHandler(instance));
            }
            return this.GetValueHandler(instance);
        }
    }

    /// <summary>
    /// 定义一个属性移植的特性。
    /// </summary>
    public interface IPropertyPopulateAttribute
    {
        /// <summary>
        /// 指定一个实例，获取指定属性的值。
        /// </summary>
        /// <param name="property">动态属性。</param>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <returns>属性的值。</returns>
        object GetValue(DynamicProperty property, object instance);

        /// <summary>
        /// 指定一个实例，设置指定属性的值。
        /// </summary>
        /// <param name="property">动态属性。</param>
        /// <param name="instance">一个实例，null 值表示静态属性。</param>
        /// <param name="value">属性的值。</param>
        void SetValue(DynamicProperty property, object instance, object value);
    }
}
