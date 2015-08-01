using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;

namespace Aoite.Serialization
{
    internal static class SerializationHelper
    {
        #region Serializable Members - 可序列化成员

        private readonly static Dictionary<Type, SerializableFieldInfo[]> FieldsCache = new Dictionary<Type, SerializableFieldInfo[]>();

        public static SerializableFieldInfo[] GetSerializableMembers(Type type)
        {
            SerializableFieldInfo[] sfields;
            var sourceType = type;
            if(!FieldsCache.TryGetValue(sourceType, out sfields))
                lock(sourceType)
                {
                    if(!FieldsCache.TryGetValue(sourceType, out sfields))
                    {
                        List<SerializableFieldInfo> list = new List<SerializableFieldInfo>();
                        int depth = 0;
                        while(type != Types.Object)
                        {
                            var fields = type.GetFields(Aoite.Reflection.Flags.InstanceAnyDeclaredOnly);
                            foreach(FieldInfo field in fields)
                            {
                                if(field == null
#if !SILVERLIGHT
 || field.IsNotSerialized
#endif
 || field.IsDefined(IgnoreAttribute.Type, false)) continue;

                                list.Add(new SerializableFieldInfo(field, depth));
                            }
                            type = type.BaseType;
                            depth++;
                        }
                        sfields = list.ToArray();
                        FieldsCache.Add(sourceType, sfields);
                    }
                }
            return sfields;
        }

        #endregion

        #region Simplify Qualified Name - 简化限定名称

        private readonly static ConcurrentDictionary<Type, string> SimplifyQualifiedNameCache = new ConcurrentDictionary<Type, string>();

        private static string CreateSimplifyQualifiedName(Type type)
        {
            StringBuilder builder = new StringBuilder();
            SimplifyQualifiedName(type, ref builder);
            return builder.ToString();
        }
        public static string SimplifyQualifiedName(Type type)
        {
            return SimplifyQualifiedNameCache.GetOrAdd(type, CreateSimplifyQualifiedName);
        }

        private static void AddNamespace(string ns, ref StringBuilder builder)
        {
            switch(ns)
            {
                case "System":
                    builder.Append('s');
                    break;
                case "System.Text":
                    builder.Append("st");
                    break;
                case "System.Data":
                    builder.Append("sd");
                    break;
                case "System.Collections":
                    builder.Append("sc");
                    break;
                case "System.Collections.Generic":
                    builder.Append("scg");
                    break;
                case "System.IO":
                    builder.Append("si");
                    break;
                default:
                    builder.Append(ns);
                    break;
            }
        }

        private static void AddTypeName(Type type, ref StringBuilder builder)
        {
            var ns = type.Namespace;
            if(!string.IsNullOrEmpty(ns))
            {
                AddNamespace(ns, ref builder);
                builder.Append('.');
                var reflectedType = type.ReflectedType;
                if(reflectedType != null)
                {
                    var lastIndex = builder.Length;
                    while(reflectedType != null)
                    {
                        builder.Insert(lastIndex, reflectedType.Name + '+');
                        reflectedType = reflectedType.ReflectedType;
                    }
                }
            }
            builder.Append(type.Name);
        }

        private static void SimplifyQualifiedName(Type type, ref StringBuilder builder)
        {
            int index = builder.Length;

            AddTypeName(type, ref builder);

            if(type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericTypes = type.GetGenericArguments();
                builder.Append('[');
                foreach(var gType in genericTypes)
                {
                    builder.Append('[');
                    SimplifyQualifiedName(gType, ref builder);
                    builder.Append(']');
                    builder.Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Append(']');
            }
            if(type.Assembly != AssemblyInfoCollection.mscorlib.Assembly)
            {
                builder.Append(',');
                builder.Append(new AssemblyName(type.Assembly.FullName).Name);
            }
            // return builder.ToString();
        }

        #endregion

        #region Recovery Qualified Name - 还原限定名称

        private readonly static ConcurrentDictionary<string, Type> RecoverySimplifyQualifiedCache = new ConcurrentDictionary<string, Type>();

        private static void AppendQualifiedName(this StringBuilder builder, string typeName, string fullname)
        {
            builder.Append(typeName);
            builder.Append(", ");
            builder.Append(fullname);
        }

        private static string GetNamespace(string ns)
        {
            switch(ns)
            {
                case "s":
                    return "System";

                case "st":
                    return "System.Text";

                case "sd":
                    return "System.Data";

                case "sc":
                    return "System.Collections";

                case "scg":
                    return "System.Collections.Generic";

                case "si":
                    return "System.IO";

                default:
                    return ns;
            }
        }

        private static Type CreateRecoveryQualifiedName(string simplifyQualifiedName)
        {
            StringBuilder builder = new StringBuilder();
            RecoveryQualifiedName(simplifyQualifiedName, ref builder);
            return AssemblyInfoCollection.GetTypeFromQualifiedName(builder.ToString());

        }

        public static Type RecoveryQualifiedName(string simplifyQualifiedName)
        {
            return RecoverySimplifyQualifiedCache.GetOrAdd(simplifyQualifiedName, CreateRecoveryQualifiedName);
        }

        private static void RecoveryQualifiedName(string simplifyQualifiedName, ref StringBuilder builder)
        {
            int builderIndex = builder.Length;

            int depth = 0;
            int index = 0;
            int firstIndex = -1;
            string assemblyName = string.Empty;
            for(int i = 0; i < simplifyQualifiedName.Length; )
            {
                var c = simplifyQualifiedName[i];
                switch(c)
                {
                    case '[':
                        if(firstIndex == -1) firstIndex = i;
                        if(depth == 0)
                        {
                            builder.Append('[');
                            index = i + 1;
                        }
                        depth++;
                        break;
                    case ']':
                        depth--;
                        if(depth == 1)
                        {
                            string str = simplifyQualifiedName.Substring(index + 1, i - index - 1);

                            builder.Append('[');
                            //builder.Append();
                            RecoveryQualifiedName(str, ref builder);
                            index = i + 2;
                        }
                        if(depth < 2)
                        {
                            builder.Append(']');
                        }
                        break;
                    case ',':
                        if(firstIndex == -1) firstIndex = i;
                        if(depth == 0)
                        {
                            assemblyName = simplifyQualifiedName.Substring(i + 1);
                            i = simplifyQualifiedName.Length;
                        }
                        else if(depth < 2)
                        {
                            builder.Append(',');
                        }
                        break;
                    default:
                        if(depth == 0)
                        {
                            builder.Append(c);
                        }
                        break;
                }
                i++;
            }
            if(string.IsNullOrEmpty(assemblyName))
            {
                assemblyName = "mscorlib";
            }

            string typeName = firstIndex == -1 ? simplifyQualifiedName : simplifyQualifiedName.Substring(0, firstIndex);
            var nsIndex = typeName.LastIndexOf('.');
            if(nsIndex > -1)
            {
                var ns = typeName.Substring(0, nsIndex);
                builder.Remove(builderIndex, nsIndex);
                builder.Insert(builderIndex, GetNamespace(ns));
            }
            var assembly = AssemblyInfoCollection.Find(assemblyName);
            builder.Append(", ");
            //TODO:如果找不到程序集，是否需要延迟加载？
            if(assembly == null)
            {
                var type = AutoMappingTypes.GetOrAdd(typeName, ObjectFactory.GetType);
                if(type == null) throw new TypeLoadException("找不到类型 {0} 的程序集。".Fmt(typeName));
                builder.Append(type.Assembly.FullName);
            }
            else builder.Append(assembly.Fullname);
        }

        #endregion

        private readonly static ConcurrentDictionary<string, Type> AutoMappingTypes = new ConcurrentDictionary<string, Type>();
    }

}
