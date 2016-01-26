using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.DI;

namespace System
{
    public interface IIocContainer : IServiceProvider
    {
        /// <summary>
        /// 获取父级服务容器。
        /// </summary>
        IIocContainer Parent { get; }
        /// <summary>
        /// 创建基于当前服务容器的子服务容器。
        /// </summary>
        /// <returns>一个新的服务容器。</returns>
        IIocContainer CreateChildLocator();
        IIocContainer Imports(Action<IServiceBuilder> callback);
        bool DisabledAutoResolving { get; set; }
        IIocContainer Add(Type expectType, bool singletonMode = false, bool promote = false);
        IIocContainer Add(Type expectType, Type actualType, bool singletonMode = false, bool promote = false);
        IIocContainer Add(Type expectType, object serviceInstance, bool promote = false);
        IIocContainer Add(Type expectType, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);

        IIocContainer Add(string name, object value, bool promote = false);
        IIocContainer Add(string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);
        IIocContainer Add(Type expectType, string name, object value, bool promote = false);
        IIocContainer Add(Type expectType, string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);

        object Get(Type expectType, params object[] lastMappingValues);
        object GetFixed(Type expectType, params object[] lastMappingValues);
        object[] GetAll(Type expectType, params object[] lastMappingValues);
        object Get(string name, params object[] lastMappingValues);
        object Get(Type expectType, string name, params object[] lastMappingValues);
        bool Contains(Type expectType, bool promote = false);
        bool Contains(string name, bool promote = false);
        bool Contains(Type expectType, string name, bool promote = false);
        void Remove(Type expectType, bool promote = false);
        void Remove(string name, bool promote = false);
        void Remove(Type expectType, string name, bool promote = false);
        void DestroyAll();
    }
}
