using Aoite.Reflection;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// 表示一个动态成员反射的工厂。
    /// </summary>
    public static class DynamicFactory
    {

        static readonly MethodInfo ChangeTypeMethod = typeof(TypeExtensions).GetMethod("ChangeType", new Type[] { Types.Type, Types.Object });
        static readonly MethodInfo GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) });
        static readonly ConcurrentDictionary<MemberInfo, DynamicMemberGetter> GetterCache = new ConcurrentDictionary<MemberInfo, DynamicMemberGetter>();
        static readonly ConcurrentDictionary<MemberInfo, DynamicMemberSetter> SetterCache = new ConcurrentDictionary<MemberInfo, DynamicMemberSetter>();
        static readonly ConcurrentDictionary<MethodInfo, DynamicMethodInvoker> MethodCache = new ConcurrentDictionary<MethodInfo, DynamicMethodInvoker>();
        static readonly ConcurrentDictionary<ConstructorInfo, DynamicConstructorHandler> ConstructorCache = new ConcurrentDictionary<ConstructorInfo, DynamicConstructorHandler>();

        /// <summary>
        /// 创建字段的获取器委托。
        /// </summary>
        /// <param name="fieldInfo">字段元数据。</param>
        /// <returns>字段获取器的委托。</returns>
        public static DynamicMemberGetter CreateFieldGetter(this FieldInfo fieldInfo)
        {
            if(fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));

            return GetterCache.GetOrAdd(fieldInfo, m =>
            {
                var emit = new EmitHelper(Types.Object, new Type[] { Types.Object }, fieldInfo.DeclaringType);
                if(fieldInfo.IsStatic) emit.ldsfld(fieldInfo)
                                           .end();
                else emit.ldarg_0
                         .castType(fieldInfo.DeclaringType)
                         .ldfld(fieldInfo)
                         .end();

                emit.boxIfValueType(fieldInfo.FieldType)
                         .ret();

                return emit.CreateDelegate<DynamicMemberGetter>();
            });
        }

        /// <summary>
        /// 创建字段的设置器委托。
        /// </summary>
        /// <param name="fieldInfo">字段元数据。</param>
        /// <returns>字段获取器的委托。</returns>
        public static DynamicMemberSetter CreateFieldSetter(this FieldInfo fieldInfo)
        {
            if(fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));

            return SetterCache.GetOrAdd(fieldInfo, m =>
            {
                var declaringType = fieldInfo.DeclaringType;

                var emit = new EmitHelper(Types.Void, new Type[] { Types.Object, Types.Object }, declaringType);
                var isStatic = fieldInfo.IsStatic;

                if(!isStatic)
                {
                    emit.ldarg_0
                        .castType(declaringType)
                        .end();
                }

                emit.ldtoken(fieldInfo.FieldType)
                    .call(GetTypeFromHandleMethod)
                    .ldarg_1
                    .call(ChangeTypeMethod)
                    .castType_any(fieldInfo.FieldType)
                    .stfld(isStatic, fieldInfo)
                    .ret()
                    .end();
                return emit.CreateDelegate<DynamicMemberSetter>();
            });
        }

        /// <summary>
        /// 创建属性的获取器委托。
        /// </summary>
        /// <param name="propertyInfo">属性元数据。</param>
        /// <returns>属性获取器的委托。</returns>
        public static DynamicMemberGetter CreatePropertyGetter(this PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            return GetterCache.GetOrAdd(propertyInfo, m =>
            {
                var declaringType = propertyInfo.DeclaringType;

                var methodInfo = propertyInfo.GetGetMethod(true);
                if(methodInfo == null) return null;

                var emit = new EmitHelper(Types.Object, new Type[] { Types.Object }, declaringType);
                var isStatic = methodInfo.IsStatic;

                if(!isStatic)
                {
                    emit.ldarg_0
                        .castType(declaringType)
                        .callvirt(methodInfo)
                        .end();
                }
                else
                {
                    emit.call(methodInfo)
                        .end();
                }

                emit.boxIfValueType(propertyInfo.PropertyType)
                    .ret();

                return emit.CreateDelegate<DynamicMemberGetter>();
            });
        }

        /// <summary>
        /// 创建属性的设置器委托。
        /// </summary>
        /// <param name="propertyInfo">属性元数据。</param>
        /// <returns>属性获取器的委托。</returns>
        public static DynamicMemberSetter CreatePropertySetter(this PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            return SetterCache.GetOrAdd(propertyInfo, m =>
            {
                var declaringType = propertyInfo.DeclaringType;
                var methodInfo = propertyInfo.GetSetMethod(true);
                if(methodInfo == null) return null;

                var emit = new EmitHelper(Types.Void, new Type[] { Types.Object, Types.Object }, declaringType);
                var isStatic = methodInfo.IsStatic;
                if(!isStatic)
                {
                    emit.ldarg_0
                        .castType(declaringType)
                        .end();
                }

                emit.ldtoken(propertyInfo.PropertyType)
                    .call(GetTypeFromHandleMethod)
                    .ldarg_1
                    .call(ChangeTypeMethod)
                    .castType_any(propertyInfo.PropertyType)
                    .call(isStatic, methodInfo)
                    .ret()
                    .end();

                return emit.CreateDelegate<DynamicMemberSetter>();
            });
        }

        static void CreateParameterLocals(int argsIndex, EmitHelper emit, ParameterInfo[] parameterInfos, int parameterLength, LocalBuilder[] parameterLocals, ref bool hasByRef)
        {
            for(int i = 0; i < parameterLength; i++)
            {
                var parameterInfo = parameterInfos[i];
                var parameterType = parameterInfo.ParameterType;
                if(parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                    hasByRef = true;
                }
                var local = emit.DeclareLocal(parameterType);
                parameterLocals[i] = local;
                emit.ldtoken(parameterType)
                    .call(GetTypeFromHandleMethod)
                    .ldarg(argsIndex)                //- 取出参数1 也就是 object[]
                    .ldc_i4_(i)             //- 指定索引号 —— n
                    .ldelem_ref             //- 取出索引元素 object[n]
                    .call(ChangeTypeMethod)
                    .castType_any(parameterType)
                    .stloc(local)
                    .end();
            }
        }
        static void LoadParameterLocals(EmitHelper emit, ParameterInfo[] parameterInfos, int parameterLength, LocalBuilder[] parameterLocals)
        {
            for(int i = 0; i < parameterLength; i++)
            {
                if(parameterInfos[i].ParameterType.IsByRef)
                    emit.ldloca(parameterLocals[i])
                        .end();
                else
                    emit.ldloc(parameterLocals[i])
                        .end();
            }
        }
        static void SaveParameters(int argsIndex, EmitHelper emit, ParameterInfo[] parameterInfos, int parameterLength, LocalBuilder[] parameterLocals)
        {
            for(int i = 0; i < parameterLength; i++)
            {
                if(parameterInfos[i].ParameterType.IsByRef)
                {
                    var local = parameterLocals[i];
                    emit.ldarg(argsIndex)                            //- 取出参数1 也就是 object[]
                        .ldc_i4(i)                          //- 指定索引号 —— 0
                        .ldloc(local)                       //- 加载指定索引的数组元素
                        .boxIfValueType(local.LocalType)    //- 尝试装箱  paramTypes[i]
                        .stelem_ref                         //- 赋值给 ref 或  out
                        .end();
                }
            }
        }

        /// <summary>
        /// 创建方法的调用委托。
        /// </summary>
        /// <param name="methodInfo">方法元数据。方法不能是一个尚未构造泛型参数的方法</param>
        /// <returns>方法调用的委托。</returns>
        public static DynamicMethodInvoker CreateMethodInvoker(this MethodInfo methodInfo)
        {
            if(methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            if(methodInfo.IsGenericMethodDefinition) throw new ArgumentException("不支持尚未构造泛型参数的方法。", nameof(methodInfo));

            return MethodCache.GetOrAdd(methodInfo, m =>
            {
                var declaringType = m.DeclaringType;

                var emit = new EmitHelper(Types.Object, new Type[] { Types.Object, Types.ObjectArray }, declaringType);
                var isStatic = m.IsStatic;
                var returnType = m.ReturnType;                      //- 方法的返回类型
                var parameterInfos = m.GetParameters();  //- 方法的参数集合
                var parameterLength = parameterInfos.Length;
                var parameterLocals = new LocalBuilder[parameterLength];
                var hasByRef = false;

                CreateParameterLocals(1, emit, parameterInfos, parameterLength, parameterLocals, ref hasByRef);

                if(!isStatic)
                {
                    emit.ldarg_0
                        .castType_any(declaringType)
                        .end();
                }

                LoadParameterLocals(emit, parameterInfos, parameterLength, parameterLocals);

                emit.call(isStatic, m);

                if(returnType == typeof(void))
                {
                    emit.ldnull
                        .end();
                }
                else
                {
                    emit.boxIfValueType(returnType)
                        .end();
                }

                if(hasByRef) SaveParameters(1, emit, parameterInfos, parameterLength, parameterLocals);

                emit.ret().end();

                return emit.CreateDelegate<DynamicMethodInvoker>();

            });
        }

        /// <summary>
        /// 创建指定 <paramref name="constructorInfo"/> 的动态构造函数。
        /// </summary>
        /// <param name="constructorInfo">构造函数的元数据。</param>
        /// <returns>绑定到实例构造函数的委托。</returns>
        public static DynamicConstructorHandler CreateConstructorHandler(this ConstructorInfo constructorInfo)
        {
            if(constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
            return ConstructorCache.GetOrAdd(constructorInfo, m =>
            {
                var declaringType = m.DeclaringType;

                var emit = new EmitHelper(Types.Object, new Type[] { Types.ObjectArray }, declaringType);

                var parameterInfos = m.GetParameters();
                var parameterLength = parameterInfos.Length;
                var parameterLocals = new LocalBuilder[parameterLength];
                var hasByRef = false;

                CreateParameterLocals(0, emit, parameterInfos, parameterLength, parameterLocals, ref hasByRef);
                LoadParameterLocals(emit, parameterInfos, parameterLength, parameterLocals);

                emit.newobj(m)
                    .boxIfValueType(declaringType)
                    .end();

                if(hasByRef) SaveParameters(0, emit, parameterInfos, parameterLength, parameterLocals);

                emit.ret().end();
                return emit.CreateDelegate<DynamicConstructorHandler>();

            });
        }

        /// <summary>
        /// 创建指定 <paramref name="type"/> 的动态构造函数。
        /// </summary>
        /// <param name="type">构造函数的定义类。</param>
        /// <param name="types">表示需要的构造函数的参数个数、顺序和类型的 <see cref="Type"/> 对象的数组。- 或 -<see cref="Type"/> 对象的空数组，用于获取不带参数的构造函数。这样的空数组由 static 字段 <see cref="Type.EmptyTypes"/> 提供。</param>
        /// <returns>绑定到实例构造函数的委托。</returns>
        public static DynamicConstructorHandler CreateConstructorHandler(this Type type, params Type[] types)
        {
            var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, types, null);
            if(ctor == null) throw new MissingMemberException($"找不到指定类型 {type.FullName} 的构造函数，可能是参数的数量或类型不匹配。");
            return CreateConstructorHandler(ctor);
        }

        /// <summary>
        /// 创建指定类型的实例对象。
        /// </summary>
        /// <param name="type">创建实例的类型。</param>
        /// <returns><paramref name="type"/> 的新实例。</returns>
        public static object CreateUninitializedInstance(this Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }

        /// <summary>
        /// 创建指定类型的实例对象。
        /// </summary>
        /// <param name="type">创建实例的类型。</param>
        /// <param name="args">构造函数的参数。</param>
        /// <returns><paramref name="type"/> 的新实例。</returns>
        public static object CreateInstance(this Type type, params object[] args)
        {
            return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, args, null);
        }

        /// <summary>
        /// 创建指定类型的实例对象。
        /// </summary>
        /// <typeparam name="T">创建实例的类型。</typeparam>
        /// <param name="args">构造函数的参数。</param>
        /// <returns><typeparamref name="T"/> 的新实例。</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

    }
}
