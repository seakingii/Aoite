using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DependencyInjection
{

    interface IIocContainer2 : IServiceProvider
    {
        /// <summary>
        /// 获取父级服务容器。
        /// </summary>
        IIocContainer2 Parent { get; }
        /// <summary>
        /// 创建基于当前服务容器的子服务容器。
        /// </summary>
        /// <returns>一个新的服务容器。</returns>
        IIocContainer2 CreateChildLocator();
        bool DisabledAutoResolving { get; set; }
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
    sealed class ServiceLocator : IIocContainer2
    {
        private bool _hasParent;
        public bool DisabledAutoResolving { get; set; }
        public ServiceLocator Parent { get; }

        IIocContainer2 IIocContainer2.Parent => this.Parent;

        /// <summary>
        /// 表示解析映射类型时发生。
        /// </summary>
        public event MapResolveEventHandler MapResolve;

        internal ICallSite OnMapResolve(Type expectType)
        {
            var e = ObjectFactory.InternalOnMapResolve(MapResolve, this, expectType)
                ?? ObjectFactory.InternalOnMapResolve(null, this, expectType);

            if(e != null)
            {
                if(e.Callback == null) throw new ArgumentNullException(nameof(e.Callback));
                return this.CreateCallSite(e.Litetime, () => e.Callback);
            }
            return null;
        }

        private ICallSite CreateCallSite(ServiceLifetime lifetime, Func<InstanceCreatorCallback> callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback));
            switch(lifetime)
            {
                case ServiceLifetime.Transient: return new TransientCallSite(callback);
                case ServiceLifetime.Scoped: return new ScopedCallSite(callback);
                case ServiceLifetime.Singleton: return new SingletonCallSite(callback);
                case ServiceLifetime.LastMapping:
                default:
                    throw new NotSupportedException($"不支持服务生命周期类型{lifetime}。");
            }
        }

        #region Create CallSite

        internal Type FindActualType(Type expectType)
        {
            var defaultMappingAttr = expectType.GetAttribute<DefaultMappingAttribute>();
            if(defaultMappingAttr != null) return defaultMappingAttr.ActualType;

            //- 不支持基类或值类型
            if((expectType.IsClass && expectType.IsAbstract) || expectType.IsValueType) return null;
            //- 是一个普通类
            if(!expectType.IsInterface) return expectType;
            //- 否则就是一个接口

            //- 获取通过接口名称智能分析出来的所有名称限定符
            var fullNames = MapFilter.GetAllActualType(expectType);

            //- 从智能映射表中获取（当前程序集）
            foreach(var fullName in fullNames)
            {
                var actualType = expectType.Assembly.GetType(fullName, false);
                if(actualType != null) return actualType;
            }

            //- 从智能映射表中获取（所有程序集）
            return MapFilter.FindActualType(ObjectFactory.AllTypes, expectType, fullNames);
        }

        private bool FindCallSite(ServiceLocator locator, Type expectType, Type actualType, Type pType, string pName, bool hasExpectedType, bool isSimpleType, out ICallSite callSite)
        {
            //- 优先级1：目标类型+参数名
            if(locator.FindCallSite(actualType, pName, out callSite)) return true;
            //- 优先级2：预期类型+参数名
            if(hasExpectedType && locator.FindCallSite(expectType, pName, out callSite)) return true;
            //- 优先级3：参数名
            if(!isSimpleType && locator.FindCallSite(pType, out callSite)) return true;

            return false;
        }

        private bool FindCallSite(Type expectType, Type actualType, Type pType, string pName, bool hasExpectedType, bool isSimpleType, out ICallSite callSite)
        {
            if(this.FindCallSite(this, expectType, actualType, pType, pName, hasExpectedType, isSimpleType, out callSite)) return true;
            if(this._hasParent && this.FindCallSite(this.Parent, expectType, actualType, pType, pName, hasExpectedType, isSimpleType, out callSite)) return true;

            var parameterActualType = this.FindActualType(pType);

            if(this.FindCallSite(this, expectType, actualType, parameterActualType, pName, hasExpectedType, isSimpleType, out callSite)) return true;
            if(this._hasParent && this.FindCallSite(this.Parent, expectType, actualType, parameterActualType, pName, hasExpectedType, isSimpleType, out callSite)) return true;

            return false;
        }

        internal InstanceCreatorCallback CreateCallback(Type expectType, Type actualType)
        {
            this.TestActualType(actualType);

            var ctors = actualType.GetConstructors(Aoite.Reflection.Flags.InstanceAnyVisibility);
            if(ctors.Length > 1) ctors = ctors.OrderBy(ctor => ctor.GetAttribute<OrderMappingAttribute>()?.Order ?? 0).ToArray();

            var hasExpectedType = expectType != actualType;
            List<Exception> exceptions = new List<Exception>();
            foreach(var ctor in ctors)
            {
                var ps = ctor.GetParameters();
                if(ps.Length == 0) return lmvs => Activator.CreateInstance(actualType, true);

                var callSites = new ICallSite[ps.Length];
                for(int i = 0, lastMappingIndex = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    var pType = p.ParameterType;
                    var isSimpleType = pType.IsSimpleType();
                    var pName = p.Name;
                    ICallSite callSite = null;
                    if(p.IsDefined(LastMappingAttribute.Type, true)
                        || (pType.IsDefined(LastMappingAttribute.Type, true) && !p.IsDefined(IgnoreAttribute.Type, true)))
                    {
                        callSite = new LastMappingCallSite(actualType, p, lastMappingIndex++);
                    }
                    else if(!this.FindCallSite(expectType, actualType, pType, pName, hasExpectedType, isSimpleType, out callSite))
                    {
                        exceptions.Add(new ArgumentException($"类型{actualType.FullName}()：构造函数的参数“{pName}”尚未配置映射，并且自动分析失败。", pName));
                        callSites = null;
                        break;
                    }

                    callSites[i] = callSite;
                }

                if(callSites != null)
                {
                    var ctorHandler = DynamicFactory.CreateConstructorHandler(ctor);
                    return lmps => ctorHandler(callSites.Each(cs => cs.Invoke(lmps)));
                }
            }
            throw new AggregateException(actualType.FullName + "没有在映射列表中找到匹配的构造函数。错误内容可以详见 System.Exception.Data 字典。", exceptions);
        }

        internal List<ICallSite> CreateCallSites(Type expectType) => new List<ICallSite>(1) { this.CreateCallSite(expectType, null) };
        internal ICallSite CreateCallSite(Type expectType, Type actualType)
        {
            var lifetime = ServiceLifetime.Transient;
            var sla = expectType.GetAttribute<ServiceLifetimeAttribute>();
            if(sla != null) lifetime = sla.Lifetime;

            Func<InstanceCreatorCallback> callback;
            if(actualType == null)
            {
                var callSite = this.OnMapResolve(expectType);
                if(callSite != null) return callSite;

                callback = () => this.CreateCallback(expectType, this.ForceFindActualType(expectType));
            }
            else
            {
                callback = () => this.CreateCallback(expectType, actualType);
            }
            return this.CreateCallSite(lifetime, callback);
        }

        internal Type ForceFindActualType(Type expectType)
        {
            var actualType = this.FindActualType(expectType);
            if(actualType == null) throw new NotSupportedException($"预期类型“{expectType.FullName}”找不到匹配的目标类型。可能原因：1、这是一个值类型；2、没有注册接口或基类的映射关联；3、通过默认规则没有找到接口的相同程序集中的关联类型。");
            this.TestActualType(actualType);
            return actualType;
        }

        internal ServiceLocator TestActualType(Type actualType)
        {
            if(actualType == null) throw new ArgumentNullException(nameof(actualType));
            if(actualType.IsAbstract || actualType.IsInterface || actualType.IsValueType)
                throw new ArgumentException($"类型“{actualType.FullName}”映射失败，不允许将基类、接口或值类型作为映射关联的目标类型。", nameof(actualType));
            return this;
        }

        #endregion

        public ServiceLocator() { }

        /// <summary>
        /// 创建基于当前服务容器的子服务容器。
        /// </summary>
        /// <returns>一个新的服务容器。</returns>
        public IIocContainer2 CreateChildLocator()
        {
            return new ServiceLocator(this);
        }

        internal ServiceLocator(ServiceLocator parent)
        {
            if(parent == null) throw new ArgumentNullException(nameof(parent));
            this.Parent = parent;
            this._hasParent = true;
        }

        static void AppendToLocator(ServiceLocator locator, ServiceBuilder builder)
        {
            if(builder.TypeBinders.IsValueCreated)
            {
                foreach(var item in builder.TypeBinders.Value)
                {
                    locator.CacheType.AddOrUpdate(item.ExpectType, t => new List<ICallSite>(1) { item.CallSite }, (t, list) =>
                    {
                        if(item.Overwrite) list.Clear();
                        list.Insert(0, item.CallSite);
                        list.TrimExcess();
                        return list;
                    });
                }
            }

            if(builder.ValueBinders.IsValueCreated)
            {
                foreach(var item in builder.ValueBinders.Value)
                {
                    var type = item.ExpectType ?? EmptyType;
                    locator.CacheTypeName.AddOrUpdate(type, t =>
                    {
                        var dict = new ConcurrentDictionary<string, ICallSite>();
                        dict[item.Name] = item.CallSite;
                        return dict;
                    }, (t, dict) =>
                    {
                        dict[item.Name] = item.CallSite;
                        return dict;
                    });

                }
            }
        }

        public ServiceLocator Imports(Action<IServiceBuilder> callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback));
            using(var builder = new ServiceBuilder(this))
            {
                callback(builder);

                var locator = this;
                do
                {
                    AppendToLocator(locator, builder);
                } while(builder.IsPromote && locator._hasParent && (locator = locator.Parent) != null);
            }
            return this;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return this.Get(serviceType);
        }

        #region FindCallSite/s

        class EmptyTypeClass { }
        private readonly static Type EmptyType = typeof(EmptyTypeClass);

        public bool FindCallSite(string name, out ICallSite callSite)
        {
            return this.FindCallSite(EmptyType, name, out callSite);
        }

        public bool FindCallSite(Type expectType, out ICallSite callSite)
        {
            List<ICallSite> callSites;
            if(CacheType.TryGetValue(expectType, out callSites))
            {
                callSite = callSites.FirstOrDefault();
                return true;
            }
            callSite = null;
            return false;

        }

        public bool FindCallSites(Type expectType, out List<ICallSite> callSites)
        {
            return CacheType.TryGetValue(expectType, out callSites);
        }

        public bool FindCallSite(Type expectType, string name, out ICallSite callSite)
        {
            ConcurrentDictionary<string, ICallSite> nameCallSites;
            if(CacheTypeName.TryGetValue(expectType, out nameCallSites) && nameCallSites.TryGetValue(name, out callSite)) return true;
            callSite = null;
            return false;
        }

        #endregion

        #region Get

        public object Get(Type expectType, params object[] lastMappingValues)
        {
            if(this.DisabledAutoResolving) return this.GetFixed(expectType, lastMappingValues);

            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            return CacheType.GetOrAdd(expectType, CreateCallSites).FirstOrDefault().Invoke(lastMappingValues);
        }

        public object GetFixed(Type expectType, params object[] lastMappingValues)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            ICallSite callSite;
            if(this.FindCallSite(expectType, out callSite)) return callSite.Invoke(lastMappingValues);
            return null;
        }

        public object[] GetAll(Type expectType, params object[] lastMappingValues)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            List<ICallSite> callSites;
            if(this.FindCallSites(expectType, out callSites))
            {
                return (from callSite in callSites select callSite.Invoke(lastMappingValues)).ToArray();
            }
            return new object[0];
        }

        public object Get(string name, params object[] lastMappingValues) => this.Get(EmptyType, name, lastMappingValues);

        public object Get(Type expectType, string name, params object[] lastMappingValues)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            ICallSite callSite;
            if(this.FindCallSite(expectType, name, out callSite)) return callSite.Invoke(lastMappingValues);
            return null;
        }

        #endregion

        #region Contains

        public bool Contains(Type expectType, bool promote = false)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));

            return CacheType.ContainsKey(expectType) || (this._hasParent && promote && this.Parent.Contains(expectType, promote));
        }
        public bool Contains(string name, bool promote = false) => this.Contains(EmptyType, name, promote);
        public bool Contains(Type expectType, string name, bool promote = false)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            ICallSite callSite;
            return this.FindCallSite(expectType, name, out callSite) || (this._hasParent && promote && this.Parent.Contains(expectType, name, promote));
        }

        #endregion

        #region Remove

        public void Remove(Type expectType, bool promote = false)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));

            List<ICallSite> lists;
            CacheType.TryRemove(expectType, out lists);
            if(this._hasParent && promote) this.Parent.Remove(expectType, promote);
        }

        public void Remove(string name, bool promote = false) => this.Remove(EmptyType, name, promote);

        public void Remove(Type expectType, string name, bool promote = false)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            ConcurrentDictionary<string, ICallSite> dict;
            using(GA.Locking(expectType))
            {
                if(CacheTypeName.TryGetValue(expectType, out dict))
                {
                    ICallSite callSite;
                    dict.TryRemove(name, out callSite);
                }
            }
            if(this._hasParent && promote) this.Parent.Remove(expectType, name, promote);
        }

        #endregion

        /// <summary>
        /// 销毁所有的映射。
        /// </summary>
        public void DestroyAll()
        {
            CacheTypeName.Clear();
            CacheType.Clear();
            GC.WaitForFullGCComplete();
        }

        #region Cache Fields

        private readonly static EmbeddedTypeAwareTypeComparer serviceTypeComparer = new EmbeddedTypeAwareTypeComparer();
        /// <summary>
        /// 表示一个缓存列表，映射到指定类型的构造函数的参数名称的实例盒。
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ICallSite>> CacheTypeName
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, ICallSite>>(serviceTypeComparer);
        /// <summary>
        /// 表示一个缓存列表，映射对应类型的实例盒。
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<ICallSite>> CacheType
            = new ConcurrentDictionary<Type, List<ICallSite>>(serviceTypeComparer);

        private sealed class EmbeddedTypeAwareTypeComparer : IEqualityComparer<Type>
        {
            // Methods
            public bool Equals(Type x, Type y)
            {
                return x.IsEquivalentTo(y);
            }

            public int GetHashCode(Type obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        #endregion
    }
}
