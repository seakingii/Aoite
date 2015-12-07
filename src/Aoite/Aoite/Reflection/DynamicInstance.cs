using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示一个反射的实例。
    /// </summary>
    public class DynamicInstance
    {
        const BindingFlags AllBindingFalgs = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
        /// <summary>
        /// 获取反射的类。
        /// </summary>
        public virtual Type Type { get; }
        /// <summary>
        /// 获取反射的实例。当表示静态反射时为 null 值。
        /// </summary>
        public virtual object Instance { get; }

        /// <summary>
        /// 指定静态方法的类型，初始化 <see cref="DynamicInstance"/> 类的新实例。
        /// </summary>
        /// <param name="type">反射的类。</param>
        public DynamicInstance(Type type) : this(null, type, false) { }

        /// <summary>
        /// 指定一个实例，初始化 <see cref="DynamicInstance"/> 类的新实例。
        /// </summary>
        /// <param name="instance">反射的实例。</param>
        public DynamicInstance(object instance) : this(instance, instance?.GetType()) { }

        /// <summary>
        /// 指定反射的类型和实例，初始化 <see cref="DynamicInstance"/> 类的新实例。
        /// </summary>
        /// <param name="instance">反射的实例。</param>
        /// <param name="type">反射的类。</param>
        public DynamicInstance(object instance, Type type) : this(instance, type, true) { }

        DynamicInstance(object instance, Type type, bool instanceCanBeNull)
        {
            if(instanceCanBeNull && instance == null) throw new ArgumentNullException(nameof(instance));
            if(type == null) throw new ArgumentNullException(nameof(type));
            Instance = instance;
            Type = type;
        }

        /// <summary>
        /// 动态设置指定名称的成员的值。
        /// </summary>
        /// <param name="name">成员的名称，可以是一个属性或字段。</param>
        /// <param name="value">成员的值。</param>
        public virtual void Set(string name,object value)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var memberInfo = Type.GetMember(name, AllBindingFalgs).FirstOrDefault();
            DynamicMemberSetter d = null;
            if(memberInfo != null)
            {
                if(memberInfo.MemberType == MemberTypes.Property) d = DynamicFactory.CreatePropertySetter(memberInfo as PropertyInfo);
                else if(memberInfo.MemberType == MemberTypes.Field) d = DynamicFactory.CreateFieldSetter(memberInfo as FieldInfo);
            }

            if(d == null) throw new MissingMemberException(Type.FullName, name);
            d(Instance, value);
        }
        /// <summary>
        /// 动态获取指定名称的成员的值。
        /// </summary>
        /// <param name="name">成员的名称，可以是一个属性或字段。</param>
        /// <returns>成员的值。</returns>
        public virtual object Get(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            var memberInfo = Type.GetMember(name, AllBindingFalgs).FirstOrDefault();
            DynamicMemberGetter d = null;
            if(memberInfo != null)
            {
                if(memberInfo.MemberType == MemberTypes.Property) d = DynamicFactory.CreatePropertyGetter(memberInfo as PropertyInfo);
                else if(memberInfo.MemberType == MemberTypes.Field) d = DynamicFactory.CreateFieldGetter(memberInfo as FieldInfo);
            }

            if(d == null) throw new MissingMemberException(Type.FullName, name);
            return d(Instance);
        }

        /// <summary>
        /// 动态调用指定名称的泛型方法。
        /// </summary>
        /// <param name="genericTypes">泛型</param>
        /// <param name="name">方法的名称。</param>
        /// <param name="args">方法的参数值。</param>
        /// <returns>方法执行的结果，如果方法是一个 void 签名则返回 null 值。</returns>
        public virtual object Call(Type[] genericTypes, string name, params object[] args)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if(genericTypes == null) throw new ArgumentNullException(nameof(genericTypes));
            if(genericTypes.Length == 0) return Call(name, args);

            var key = name + genericTypes.Join(t => t.Name + t.MetadataToken, ", ", "[", "]");
            var methodInfo = Type.GetMethod(name, AllBindingFalgs);
            if(methodInfo == null) throw new MissingMethodException(Type.FullName, key);
            if(!methodInfo.IsGenericMethodDefinition) throw new ArgumentException("仅支持尚未构造泛型参数的方法。", name);

            var d = DynamicFactory.CreateMethodInvoker(methodInfo.MakeGenericMethod(genericTypes));
            return d(Instance, args);
        }

        /// <summary>
        /// 动态调用指定名称的方法。
        /// </summary>
        /// <param name="name">方法的名称。</param>
        /// <param name="args">方法的参数值。</param>
        /// <returns>方法执行的结果，如果方法是一个 void 签名则返回 null 值。</returns>
        public virtual object Call(string name, params object[] args)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var methodInfo = Type.GetMethod(name, AllBindingFalgs);
            if(methodInfo == null) throw new MissingMethodException(Type.FullName, name);
            var d = DynamicFactory.CreateMethodInvoker(methodInfo);
            return d(Instance, args);
        }
    }
}
