using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;

namespace System
{

    /// <summary>
    /// 一个依赖注入与控制反转的工厂对象。
    /// </summary>
    public static partial class ObjectFactory
    {
        /// <summary>
        /// 表示解析映射类型时发生。
        /// </summary>
        public static event MapResolveEventHandler MapResolve;
        /// <summary>
        /// 表示解析后期映射类型时发生。
        /// </summary>
        public static event MapResolveEventHandler LastMappingResolve;
        /// <summary>
        /// 获取默认的全局服务容器。
        /// </summary>
        public readonly static IocContainer Global = new IocContainer();
        private readonly static ConcurrentDictionary<string, List<Type>> _AllTypes;

        //TODO: 需要在 ASP.NET 运行环境下测试一下，看是否会自动加载懒加载的程序集？
        /// <summary>
        /// 获取当前应用程序域的所有有效类型。
        /// </summary>
        public static IDictionary<string, List<Type>> AllTypes { get { return _AllTypes; } }

        /// <summary>
        /// 获取指定 <see cref="Type"/> 的完全限定名，获取匹配的  <see cref="Type"/>。
        /// </summary>
        /// <param name="fullName">完全限定名。</param>
        /// <returns>返回一个匹配的  <see cref="Type"/>，或一个 null 值。</returns>
        public static Type GetType(string fullName)
        {
            List<Type> types;
            if(_AllTypes.TryGetValue(fullName, out types) && types.Count > 0) return types[0];
            return null;
        }

        private static MapResolveEventArgs InternalOnEvent(MapResolveEventHandler handler, object sender, Type expectType)
        {
            if(handler != null)
            {
                var e = new MapResolveEventArgs(expectType);
                handler(sender, e);
                if(e.Callback == null) return null;
                return e;
            }
            return null;
        }
        internal static MapResolveEventArgs InternalOnMapResolve(MapResolveEventHandler handler, object sender, Type expectType)
        {
            return InternalOnEvent(handler ?? MapResolve, sender, expectType);
        }
        internal static MapResolveEventArgs InternalOnLastMappingResolve(MapResolveEventHandler handler, object sender, Type expectType)
        {
            return InternalOnEvent(handler ?? LastMappingResolve, sender, expectType);
        }

        static Type[] GetTypes(Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch(Exception)
            {

                return new Type[0];
            }
        }
        static ObjectFactory()
        {
            _AllTypes = new ConcurrentDictionary<string, List<Type>>((from a in AppDomain.CurrentDomain.GetAssemblies()
                                                                      let pkToken = BitConverter.ToString(a.GetName().GetPublicKeyToken())
                                                                      where IsMatchAssembly(pkToken, a)
                                                                      from t in GetTypes(a)
                                                                      where !t.Name.StartsWith("<>") && !t.IsSpecialName && !t.IsCOMObject
                                                                      group t by t.FullName into g
                                                                      select g).ToDictionary(g => g.Key, g => g.ToList()));
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        static bool IsMatchAssembly(string pkToken, Assembly a)
        {
            return pkToken != "B7-7A-5C-56-19-34-E0-89" && pkToken != "B0-3F-5F-7F-11-D5-0A-3A" && !a.IsDynamic
                     && a.ManifestModule.Name != "<In Memory Module>"
                     && !a.FullName.StartsWith("System")
                     && !a.FullName.StartsWith("Microsoft")
                     && a.Location.IndexOf("App_Web") == -1
                     && a.Location.IndexOf("App_global") == -1
                     && a.FullName.IndexOf("CppCodeProvider") == -1
                     && a.FullName.IndexOf("WebMatrix") == -1
                     && a.FullName.IndexOf("SMDiagnostics") == -1
                     && !string.IsNullOrWhiteSpace(a.Location);
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var a = args.LoadedAssembly;
            var pkToken = BitConverter.ToString(a.GetName().GetPublicKeyToken());
            if(IsMatchAssembly(pkToken, a))
            {
                var query = (from t in a.GetTypes()
                             where !t.Name.StartsWith("<>") && !t.IsSpecialName && !t.IsCOMObject
                             group t by t.FullName into g
                             select g);

                foreach(var g in query)
                {
                    var list = g.ToList();
                    _AllTypes.AddOrUpdate(g.Key
                        , key => list
                        , (key, value) =>
                        {
                            value.AddRange(list);
                            return value.Distinct().ToList();
                        });
                }

            }

        }

        private static IEnumerable<IGrouping<string, Type>> AllTypesCreateFactory()
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   let pkToken = BitConverter.ToString(a.GetName().GetPublicKeyToken())
                   where IsMatchAssembly(pkToken, a)
                   from t in a.GetTypes()
                   where !t.Name.StartsWith("<>") && !t.IsSpecialName && !t.IsCOMObject
                   group t by t.FullName into g
                   select g;
        }

        //private readonly static Mean<IEnumerable<IGrouping<string, Type>>> meanAllTypes = new Mean<IEnumerable<IGrouping<string, Type>>>(AllTypesCreateFactory);

        //static ObjectFactory()
        //{
        //    AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        //}

        //static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{

        //}
        ///// <summary>
        ///// 刷新当前应用程序域的所有有效类型。
        ///// </summary>
        //public static void RefreshAllTypes()
        //{
        //    meanAllTypes.Reset();
        //}

    }
}
