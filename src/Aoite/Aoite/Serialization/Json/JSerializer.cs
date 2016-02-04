using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;

namespace Aoite.Serialization.Json
{
    /// <summary>
    /// 表示一个高效的 Json.NET 应使用的序列化和反序列化。
    /// </summary>
    public class JSerializer
    {
        internal const string ServerTypeFieldName = "__type";
        internal const int DefaultRecursionLimit = 100;
        internal const int DefaultMaxJsonLength = 2097152;

        internal static string SerializeInternal(object o)
        {
            JSerializer serializer = new JSerializer();
            return serializer.Serialize(o);
        }

        internal static object Deserialize(JSerializer serializer, string input, Type type, int depthLimit)
        {
            if(input == null) throw new ArgumentNullException("input");
            if(input.Length > serializer.MaxJsonLength) throw new ArgumentException(AtlasWeb.JSON_MaxJsonLengthExceeded, "input");

            var o = JObjectDeserializer.BasicDeserialize(input, depthLimit, serializer);
            return JObjectConverter.ConvertObjectToType(o, type, serializer);
        }

        // INSTANCE fields/methods
        private JTypeResolver _typeResolver;
        private HashSet<JConverter> _converters;
        private int _recursionLimit;
        private int _maxJsonLength;

        /// <summary>
        /// 初始化不具有类型解析程序的 <see cref="JSerializer"/> 类的新实例。
        /// </summary>
        public JSerializer() : this(null) { }

        /// <summary>
        /// 初始化具有自定义类型解析程序的 <see cref="JSerializer"/> 类的新实例。
        /// </summary>
        /// <param name="resolver"></param>
        public JSerializer(JTypeResolver resolver)
        {
            this._typeResolver = resolver;
            this._recursionLimit = DefaultRecursionLimit;
            this._maxJsonLength = DefaultMaxJsonLength;
        }


        /// <summary>
        /// 获取或设置一个值，表示是否启用名称智能小写模式。默认为 false，表示使用原来的名称。
        /// </summary>
        public bool EnabledCamelCaseName { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="JSerializer"/> 类接受的 JSON 字符串的最大长度。
        /// </summary>
        public int MaxJsonLength
        {
            get { return this._maxJsonLength; }
            set
            {
                if(value < 1) throw new ArgumentOutOfRangeException(AtlasWeb.JSON_InvalidMaxJsonLength);
                this._maxJsonLength = value;
            }
        }

        /// <summary>
        /// 获取或设置用于约束要处理的对象级别的数量的限制。
        /// </summary>
        public int RecursionLimit
        {
            get { return this._recursionLimit; }
            set
            {
                if(value < 1) throw new ArgumentOutOfRangeException(AtlasWeb.JSON_InvalidRecursionLimit);
                this._recursionLimit = value;
            }
        }

        internal JTypeResolver TypeResolver => _typeResolver;

        /// <summary>
        /// 使用 <see cref="JSerializer"/> 实例注册自定义转换器。
        /// </summary>
        /// <param name="converters">包含要注册的自定义转换器的数组。</param>
        public void RegisterConverters(IEnumerable<JConverter> converters)
        {
            if(converters == null) throw new ArgumentNullException(nameof(converters));

            if(this._converters == null) this._converters = new HashSet<JConverter>();

            this._converters.ExceptWith(converters);
            foreach(JConverter converter in converters)
            {
                this._converters.Add(converter);
            }
        }

        internal bool ConverterExistsForType(Type t, out JConverter converter)
        {
            if(this._converters == null)
            {
                converter = null;
                return false;
            }
            converter = this._converters.FirstOrDefault(c => c.IsSupported(t));
            return converter != null;
        }

        /// <summary>
        /// 将指定的 JSON 字符串转换为对象图。
        /// </summary>
        /// <param name="input">要进行反序列化的 JSON 字符串。</param>
        /// <returns>反序列化的对象。</returns>
        public object DeserializeObject(string input)
        {
            return Deserialize(this, input, null /*type*/, RecursionLimit);
        }

        /// <summary>
        ///  将指定的 JSON 字符串转换为 T 类型的对象。
        /// </summary>
        /// <typeparam name="T">所生成对象的类型。</typeparam>
        /// <param name="input">要进行反序列化的 JSON 字符串。</param>
        /// <returns>反序列化的对象。</returns>
        public T Deserialize<T>(string input)
        {
            return (T)Deserialize(this, input, typeof(T), RecursionLimit);
        }

        /// <summary>
        ///  将 JSON 格式字符串转换为指定类型的对象。
        /// </summary>
        /// <param name="input">要反序列化的 JSON 字符串。</param>
        /// <param name="targetType">所生成对象的类型。</param>
        /// <returns>反序列化的对象。</returns>
        public object Deserialize(string input, Type targetType)
        {
            return Deserialize(this, input, targetType, RecursionLimit);
        }

        /// <summary>
        /// 将给定对象转换为指定类型。
        /// </summary>
        /// <typeparam name="T"><paramref name="obj"/> 将转换成的类型。</typeparam>
        /// <param name="obj">要转换的对象</param>
        /// <returns>已转换成目标类型的对象。</returns>
        public T ConvertToType<T>(object obj)
        {
            return (T)JObjectConverter.ConvertObjectToType(obj, typeof(T), this);
        }

        /// <summary>
        /// 将指定的对象转换成指定的类型。
        /// </summary>
        /// <param name="obj">要转换的对象。</param>
        /// <param name="targetType">对象要转换为的类型。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        public object ConvertToType(object obj, Type targetType)
        {
            return JObjectConverter.ConvertObjectToType(obj, targetType, this);
        }

        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        public string Serialize(object obj)
        {
            var sb = new StringBuilder();
            Serialize(obj, sb);
            return sb.ToString();
        }

        /// <summary>
        /// 序列化对象并将生成的 JSON 字符串写入指定的 <see cref="StringBuilder"/> 对象。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="output">用于写入 JSON 字符串的 <see cref="StringBuilder"/> 对象。</param>
        public void Serialize(object obj, StringBuilder output)
        {
            this.SerializeValue(obj, output, 0, null);
            // DevDiv Bugs 96574: Max JSON length does not apply when serializing to Javascript for ScriptDescriptors
            if(output.Length > MaxJsonLength) throw new InvalidOperationException(AtlasWeb.JSON_MaxJsonLengthExceeded);
        }

        private static void SerializeBoolean(bool o, StringBuilder sb)
        {
            sb.Append(o ? "true" : "false");
        }

        private static void SerializeUri(Uri uri, StringBuilder sb)
        {
            sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped)).Append("\"");
        }

        private static void SerializeGuid(Guid guid, StringBuilder sb)
        {
            sb.Append("\"").Append(guid.ToString()).Append("\"");
        }

        internal static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        private static void SerializeDateTime(DateTime datetime, StringBuilder sb)
        {
            // DevDiv 41127: Never confuse atlas serialized strings with dates
            // Serialized date: "\/Date(123)\/"
            sb.Append(@"""\/Date(");
            sb.Append((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
            sb.Append(@")\/""");
        }

        // Serialize custom object graph
        private void SerializeCustomObject(object o, StringBuilder sb, int depth, Hashtable objectsInUse)
        {
            var first = true;
            var type = o.GetType();
            sb.Append('{');

            // Serialize the object type if we have a type resolver
            if(TypeResolver != null)
            {
                // Only do this if the context is actually aware of this type
                var typeString = TypeResolver.ResolveTypeId(type);
                if(typeString != null)
                {
                    SerializeString(ServerTypeFieldName, sb);
                    sb.Append(':');
                    SerializeValue(typeString, sb, depth, objectsInUse);
                    first = false;
                }
            }

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            var typeMapper = TypeMapper.Create(type);
            foreach(var propertyMapper in typeMapper.Properties)
            {
                if(propertyMapper.IsIgnore) continue;
                if(!first) sb.Append(',');

                var name = propertyMapper.Name;
                if(this.EnabledCamelCaseName) name = name.ToCamelCase();
                SerializeString(name, sb);
                sb.Append(':');
                this.SerializeValue(propertyMapper.GetValue(o, false), sb, depth, objectsInUse, propertyMapper);
                first = false;
            }

            sb.Append('}');
        }

        private void SerializeDictionary(IDictionary o, StringBuilder sb, int depth, Hashtable objectsInUse)
        {
            sb.Append('{');
            bool isFirstElement = true, isTypeEntrySet = false;

            //make sure __type field is the first to be serialized if it exists
            if(o.Contains(ServerTypeFieldName))
            {
                isFirstElement = false;
                isTypeEntrySet = true;
                SerializeDictionaryKeyValue(ServerTypeFieldName, o[ServerTypeFieldName], sb, depth, objectsInUse);
            }

            foreach(DictionaryEntry entry in o)
            {
                var key = entry.Key as string;
                if(key == null) throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.JSON_DictionaryTypeNotSupported, o.GetType().FullName));

                if(isTypeEntrySet && string.Equals(key, ServerTypeFieldName, StringComparison.Ordinal))
                {
                    // The dictionay only contains max one entry for __type key, and it has been iterated
                    // through, so don't need to check for is anymore.
                    isTypeEntrySet = false;
                    continue;
                }
                if(!isFirstElement) sb.Append(',');

                this.SerializeDictionaryKeyValue(key, entry.Value, sb, depth, objectsInUse);
                isFirstElement = false;
            }
            sb.Append('}');
        }

        private void SerializeDictionaryKeyValue(string key, object value, StringBuilder sb, int depth, Hashtable objectsInUse)
        {
            SerializeString(key, sb);
            sb.Append(':');
            this.SerializeValue(value, sb, depth, objectsInUse);
        }

        private void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb, int depth, Hashtable objectsInUse)
        {
            sb.Append('[');
            var isFirstElement = true;
            foreach(object o in enumerable)
            {
                if(!isFirstElement) sb.Append(',');

                SerializeValue(o, sb, depth, objectsInUse);
                isFirstElement = false;
            }
            sb.Append(']');
        }

        private static void SerializeString(string input, StringBuilder sb)
        {
            sb.Append('"');
            sb.Append(HttpUtility.JavaScriptStringEncode(input));
            sb.Append('"');
        }

        private void SerializeValue(object o, StringBuilder sb, int depth, Hashtable objectsInUse, PropertyMapper propertyMapper = null)
        {
            if(++depth > this._recursionLimit) throw new ArgumentException(AtlasWeb.JSON_DepthLimitExceeded);

            // Check whether a custom converter is available for this type.
            JConverter converter = null;
            if(o != null && this.ConverterExistsForType(o.GetType(), out converter))
            {
                var dict = converter.Serialize(o, this);

                if(this.TypeResolver != null)
                {
                    var typeString = TypeResolver.ResolveTypeId(o.GetType());
                    if(typeString != null)
                    {
                        dict[ServerTypeFieldName] = typeString;
                    }
                }

                sb.Append(Serialize(dict));
                return;
            }

            this.SerializeValueInternal(o, sb, depth, objectsInUse, propertyMapper);
        }

        // We use this for our cycle detection for the case where objects override equals/gethashcode
        private class ReferenceComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object x, object y)
            {
                return x == y;
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }

        private void SerializeValueInternal(object o, StringBuilder sb, int depth, Hashtable objectsInUse, PropertyMapper propertyMapper)
        {
            if(o is Data.DynamicEntityValue)
            {
                this.SerializeValueInternal((o as Data.DynamicEntityValue).NameValues, sb, depth, objectsInUse, propertyMapper);
                return;
            }
            // 'null' is a special JavaScript token
            if(o == null || DBNull.Value.Equals(o))
            {
                sb.Append("null");
                return;
            }

            if(o is char) o = o.ToString();
            // Strings and chars are represented as quoted (single or double) in Javascript.
            if(o is string)
            {
                SerializeString(o as string, sb);
                return;
            }

            // Bools are represented as 'true' and 'false' (no quotes) in Javascript.
            if(o is bool)
            {
                SerializeBoolean((bool)o, sb);
                return;
            }

            // DateTimeOffset is converted to a UTC DateTime and serialized as usual.
            if(o is DateTimeOffset) o = ((DateTimeOffset)o).UtcDateTime;
            if(o is DateTime)
            {
                SerializeDateTime((DateTime)o, sb);
                return;
            }

            if(o is Guid)
            {
                SerializeGuid((Guid)o, sb);
                return;
            }

            if(o is Uri)
            {
                SerializeUri(o as Uri, sb);
                return;
            }

            // Have to special case floats to get full precision
            if(o is double)
            {
                sb.Append(((double)o).ToString("r", CultureInfo.InvariantCulture));
                return;
            }

            if(o is float)
            {
                sb.Append(((float)o).ToString("r", CultureInfo.InvariantCulture));
                return;
            }

            var type = o.GetType();
            // Deal with any server type that can be represented as a number in JavaScript
            if(type.IsPrimitive || o is decimal)
            {
                sb.Append((o as IConvertible).ToString(CultureInfo.InvariantCulture));
                return;
            }

            // Serialize enums as their integer value
            if(type.IsEnum)
            {
                // Int64 and UInt64 result in numbers too big for JavaScript
                var underlyingType = Enum.GetUnderlyingType(type);
                if(underlyingType == Types.Int64 || underlyingType == Types.UInt64)
                {
                    // DevDiv #286382 - Try to provide a better error message by saying exactly what property failed.
                    var errorMessage = AtlasWeb.JSON_InvalidEnumType;
                    if(propertyMapper != null)
                        errorMessage = string.Format(CultureInfo.CurrentCulture, AtlasWeb.JSON_CannotSerializeMemberGeneric, propertyMapper.Name, propertyMapper.Property.ReflectedType.FullName)
                            + " " + errorMessage;
                    throw new InvalidOperationException(errorMessage);
                }
                // DevDiv Bugs 154763: call ToString("D") rather than cast to int
                // to support enums that are based on other integral types
                sb.Append(((Enum)o).ToString("D"));
                return;
            }

            try
            {
                // The following logic performs circular reference detection
                if(objectsInUse == null)
                {
                    // Create the table on demand
                    objectsInUse = new Hashtable(new ReferenceComparer());
                }
                else if(objectsInUse.ContainsKey(o))
                {
                    // If the object is already there, we have a circular reference!
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.JSON_CircularReference, type.FullName));
                }
                // Add the object to the objectsInUse
                objectsInUse.Add(o, null);

                // Dictionaries are represented as Javascript objects.  e.g. { name1: val1, name2: val2 }
                var od = o as IDictionary;
                if(od != null)
                {
                    SerializeDictionary(od, sb, depth, objectsInUse);
                    return;
                }

                // Enumerations are represented as Javascript arrays.  e.g. [ val1, val2 ]
                var oenum = o as IEnumerable;
                if(oenum != null)
                {
                    SerializeEnumerable(oenum, sb, depth, objectsInUse);
                    return;
                }

                // Serialize all public fields and properties.
                SerializeCustomObject(o, sb, depth, objectsInUse);
            }
            finally
            {
                // Remove the object from the circular reference detection table
                if(objectsInUse != null)
                {
                    objectsInUse.Remove(o);
                }
            }
        }
    }
}
