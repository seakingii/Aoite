using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.DI;

namespace System
{
    /// <summary>
    /// 定义一个服务容器。
    /// </summary>
    public interface IIocContainer : IServiceProvider
    {
        /// <summary>
        /// 获取父级服务容器。
        /// </summary>
        IIocContainer Parent { get; }
        /// <summary>
        /// 创建基于当前服务容器的子服务容器。
        /// </summary>
        /// <returns>新的服务容器。</returns>
        IIocContainer CreateChildContainer();
        /// <summary>
        /// 批量构建服务。
        /// </summary>
        /// <param name="callback">构建回调函数。</param>
        /// <returns>当前服务容器。</returns>
        IIocContainer Imports(Action<IServiceBuilder> callback);
        /// <summary>
        /// 获取或设置一个值，表示是都禁用自动智能解析的功能。
        /// </summary>
        bool DisabledAutoResolving { get; set; }
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="expectType">要添加的预期服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="expectType">要添加的预期服务类型。</param>
        /// <param name="actualType">实际的服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, Type actualType, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="expectType">要添加的预期服务类型。</param>
        /// <param name="serviceInstance">要添加的服务的实例。 此对象必须实现 <paramref name="expectType"/> 参数所指示的类型或从其继承。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, object serviceInstance, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="expectType">要添加的预期服务类型。</param>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(string name, object value, bool promote = false);
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的预期服务类型的构造函数。
        /// </summary>
        /// <param name="expectType">关联的预期服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, string name, object value, bool promote = false);
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的预期服务类型的构造函数。
        /// </summary>
        /// <param name="expectType">关联的预期服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        IIocContainer Add(Type expectType, string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);

        /// <summary>
        /// 获取指定类型的服务对象。
        /// </summary>
        /// <param name="expectType">一个对象，它指定要获取的服务对象的类型。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns><paramref name="expectType"/> 类型的服务对象。- 或 -如果没有 <paramref name="expectType"/> 类型的服务对象，则为 null。</returns>
        object Get(Type expectType, params object[] lastMappingValues);
        /// <summary>
        /// 从手工注册服务列表中，获取指定类型的服务对象。
        /// </summary>
        /// <param name="expectType">一个对象，它指定要获取的服务对象的类型。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns><paramref name="expectType"/> 类型的服务对象。- 或 -如果没有 <paramref name="expectType"/> 类型的服务对象，则为 null。</returns>
        object GetFixed(Type expectType, params object[] lastMappingValues);
        /// <summary>
        /// 获取指定类型的所有服务对象。
        /// </summary>
        /// <param name="expectType">一个对象，它指定要获取的服务对象的类型。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns><paramref name="expectType"/> 类型的所有服务对象。</returns>
        IEnumerable<object> GetAll(Type expectType, params object[] lastMappingValues);

        /// <summary>
        /// 获取指定参数名称的值。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        object Get(string name, params object[] lastMappingValues);
        /// <summary>
        /// 获取指定关联的预期服务类型和参数名称的值。
        /// </summary>
        /// <param name="expectType">关联的预期服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        object Get(Type expectType, string name, params object[] lastMappingValues);
        /// <summary>
        /// 查找服务容器是否包含指定的预期服务类型。
        /// </summary>
        /// <param name="expectType">要查找的预期服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool Contains(Type expectType, bool promote = false);
        /// <summary>
        /// 查找服务容器是否包含指定的参数。
        /// </summary>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool Contains(string name, bool promote = false);
        /// <summary>
        /// 查找服务容器是否包含指定关联的预期服务类型指定的参数。
        /// </summary>
        /// <param name="expectType">关联的预期服务类型。</param>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool Contains(Type expectType, string name, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定的预期服务类型。
        /// </summary>
        /// <param name="expectType">要移除的预期服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void Remove(Type expectType, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定的参数。
        /// </summary>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void Remove(string name, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定关联的预期服务类型的参数。
        /// </summary>
        /// <param name="expectType">关联的预期服务类型。</param>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void Remove(Type expectType, string name, bool promote = false);
        /// <summary>
        /// 销毁所有的映射。
        /// </summary>
        void DestroyAll();
    }
}
