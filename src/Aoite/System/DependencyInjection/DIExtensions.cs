using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DependencyInjection
{
    static class DIExtensions
    {
        public static ITypeServiceBinder Use<T>(this IServiceBuilder binder) => binder.Use(typeof(T));
        public static ITypeServiceBinder UseRange<T>(this IServiceBuilder binder) => binder.UseRange(typeof(T));
        public static IValueServiceBinder Use<T>(this IServiceBuilder binder, string name) => binder.Use(typeof(T), name);

        public static IServiceBuilder Transient<T>(this ITypeServiceBinder binder) => binder.Transient(typeof(T));
        public static IServiceBuilder Singleton<T>(this ITypeServiceBinder binder) => binder.Singleton(typeof(T));
        public static IServiceBuilder Scoped<T>(this ITypeServiceBinder binder) => binder.Scoped(typeof(T));
        public static IServiceBuilder As<T>(this ITypeServiceBinder binder) => binder.As(typeof(T));


        public static T Get<T>(this IIocContainer2 container, params object[] lastMappingValues) => (T)container.Get(typeof(T), lastMappingValues);
        public static T GetFixed<T>(this IIocContainer2 container, params object[] lastMappingValues) => (T)container.GetFixed(typeof(T), lastMappingValues);
        public static T[] GetAll<T>(this IIocContainer2 container, params object[] lastMappingValues) => container.GetAll(typeof(T), lastMappingValues).Cast<T>().ToArray();
        public static object Get<T>(this IIocContainer2 container, string name, params object[] lastMappingValues) => container.Get(typeof(T), name, lastMappingValues);

        public static bool Contains<T>(this IIocContainer2 container, bool promote = false) => container.Contains(typeof(T), promote);
        public static bool Contains<T>(this IIocContainer2 container, string name, bool promote = false) => container.Contains(typeof(T), name, promote);
        public static void Remove<T>(this IIocContainer2 container, bool promote = false) => container.Remove(typeof(T), promote);
        public static void Remove<T>(this IIocContainer2 container, string name, bool promote = false) => container.Remove(typeof(T), name, promote);

    }
}
