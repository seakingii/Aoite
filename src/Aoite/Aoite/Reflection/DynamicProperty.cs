using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个动态的属性。
    /// </summary>
    public class DynamicProperty
    {
        private PropertyInfo _Property;
        /// <summary>
        /// 获取成员的属性元数据。
        /// </summary>
        public virtual PropertyInfo Property { get { return this._Property; } }

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
            if(propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            this._Property = propertyInfo;
            this.lazy_setValue = new Lazy<DynamicMemberSetter>(this._Property.CreatePropertySetter);
            this.lazy_getValue = new Lazy<DynamicMemberGetter>(this._Property.CreatePropertyGetter);
        }
    }
}
