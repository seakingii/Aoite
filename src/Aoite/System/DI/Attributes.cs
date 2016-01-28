using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.DI;

namespace System
{
    /// <summary>
    /// 表示服务生命周期的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ServiceLifetimeAttribute : Attribute
    {
        /// <summary>
        /// 获取服务的生命周期。
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// 提供服务的生命周期，初始化一个 <see cref="ServiceLifetimeAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="lifetime">服务的生命周期。</param>
        public ServiceLifetimeAttribute(ServiceLifetime lifetime)
        {
            switch(lifetime)
            {
                case ServiceLifetime.Transient:
                case ServiceLifetime.Scoped:
                case ServiceLifetime.Singleton:
                    break;
                default:
                    throw new NotSupportedException($"不支持服务生命周期类型{lifetime}。");
            }

            this.Lifetime = lifetime;
        }
    }

    /// <summary>
    /// 表示依赖注入采用单例模式的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class SingletonMappingAttribute : ServiceLifetimeAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="SingletonMappingAttribute"/> 类的新实例。
        /// </summary>
        public SingletonMappingAttribute() : base(ServiceLifetime.Singleton) { }
    }

    /// <summary>
    /// 表示依赖注入采用范围模式的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ScopedMappingAttribute : ServiceLifetimeAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="ScopedMappingAttribute"/> 类的新实例。
        /// </summary>
        public ScopedMappingAttribute() : base(ServiceLifetime.Scoped) { }
    }

    /// <summary>
    /// 表示依赖注入构造函数的解析排序。
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class OrderMappingAttribute : Attribute
    {
        /// <summary>
        /// 设置或获取一个值，表示构造函数解析排序。数值越小优先解析。默认为 1。
        /// </summary>
        public int Order { get; set; } = 1;
    }

    /// <summary>
    /// 表示一个默认映射的实际服务类型的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DefaultMappingAttribute : Attribute
    {
        private Type _ActualType;
        /// <summary>
        /// 获取默认映射的实际数据类型。
        /// </summary>
        public Type ActualType { get { return _ActualType; } }

        /// <summary>
        /// 初始化一个 <see cref="DefaultMappingAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="actualType">默认映射的实际数据类型。</param>
        public DefaultMappingAttribute(Type actualType)
        {
            if(actualType == null) throw new ArgumentNullException(nameof(actualType));
            this._ActualType = actualType;
        }
    }


    /// <summary>
    /// 表示依赖注入后期映射的特性，如果后期依赖元素在非 <see cref="AttributeTargets.Parameter"/>，而又考虑忽略，可以采用 <see cref="IgnoreAttribute"/> 对其进行忽略后期绑定。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LastMappingAttribute : Attribute
    {
        internal static readonly Type Type = typeof(LastMappingAttribute);
    }
}
