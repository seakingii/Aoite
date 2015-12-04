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
        public virtual DynamicMemberSetter SetValue { get { return this.lazy_setValue.Value; } }

        private Lazy<DynamicMemberGetter> lazy_getValue;
        /// <summary>
        /// 获取属性的读取器。
        /// </summary>
        public virtual DynamicMemberGetter GetValue { get { return this.lazy_getValue.Value; } }

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
        }
    }
}
