namespace System
{
    /// <summary>
    /// 基本数据类型的集合。
    /// </summary>
    public static class Types
    {
        #region 框架类型

        /// <summary>
        /// 表示 <see cref="Aoite.Data.DbEngineManager"/> 的类型。
        /// </summary>
        public static readonly Type DbEngineManager = typeof(Aoite.Data.DbEngineManager);
        /// <summary>
        /// 表示 <see cref="IDbEngine"/> 的类型。
        /// </summary>
        public static readonly Type IDbEngine = typeof(IDbEngine);
        /// <summary>
        /// 表示 <see cref="Result&lt;TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GResult = typeof(System.Result<>);
        /// <summary>
        /// 表示 <see cref="Result"/> 的类型。
        /// </summary>
        public static readonly Type Result = typeof(System.Result);
        /// <summary>
        /// 表示 <see cref="BinaryValue"/> 的类型。
        /// </summary>
        public static readonly Type BinaryValue = typeof(System.BinaryValue);

        /// <summary>
        /// 表示 <see cref="Aoite.Serialization.ICustomSerializable"/> 的类型。
        /// </summary>
        public static readonly Type ISerializable = typeof(Aoite.Serialization.ICustomSerializable);

        #endregion

        #region 其他类型

        /// <summary>
        /// 表示 <see cref="Convert"/> 的类型。
        /// </summary>
        public static readonly Type Convert = typeof(System.Convert);
        /// <summary>
        /// 表示 <see cref="Delegate"/> 的类型。
        /// </summary>
        public static readonly Type Delegate = typeof(System.Delegate);
        /// <summary>
        /// 表示 <see cref="Enum"/> 的类型。
        /// </summary>
        public static readonly Type Enum = typeof(Enum);
        /// <summary>
        /// 表示 <see cref="Uri"/> 的类型。
        /// </summary>
        public static readonly Type Uri = typeof(Uri);
        /// <summary>
        /// 表示 <see cref="Exception"/> 的类型。
        /// </summary>
        public static readonly Type Exception = typeof(System.Exception);
        /// <summary>
        /// 表示 <see cref="IConvertible"/> 的类型。
        /// </summary>
        public static readonly Type IConvertible = typeof(IConvertible);
        /// <summary>
        /// 表示 <see cref="IDisposable"/> 的类型。
        /// </summary>
        public static readonly Type IDisposable = typeof(System.IDisposable);
        /// <summary>
        /// 表示 <see cref="IO.MemoryStream"/> 的类型。
        /// </summary>
        public static readonly Type MemoryStream = typeof(System.IO.MemoryStream);
        /// <summary>
        /// 表示 <see cref="IO.Stream"/> 的类型。
        /// </summary>
        public static readonly Type Stream = typeof(System.IO.Stream);
        /// <summary>
        /// 表示 <see cref="Nullable&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type Nullable = typeof(Nullable<>);
        /// <summary>
        /// 表示 <see cref="Text.RegularExpressions.Regex"/> 的类型。
        /// </summary>
        public static readonly Type Regex = typeof(System.Text.RegularExpressions.Regex);
        /// <summary>
        /// 表示 void 的类型。
        /// </summary>
        public static readonly Type Void = typeof(void);
        /// <summary>
        /// 表示 <see cref="Type"/> 的类型。
        /// </summary>
        public static readonly Type Type = typeof(Type);
        /// <summary>
        /// 表示 <see cref="Type"/>[] 的类型。
        /// </summary>
        public static readonly Type TypeArray = typeof(Type[]);

        #endregion

        #region 集合类型

        /// <summary>
        /// 表示 <see cref="Collections.ArrayList"/> 的类型。
        /// </summary>
        public static readonly Type ArrayList = typeof(System.Collections.ArrayList);
        /// <summary>
        /// 表示 <see cref="Array"/> 的类型。
        /// </summary>
        public static readonly Type Array = typeof(System.Array);
        /// <summary>
        /// 表示 <see cref="Collections.IEnumerator"/> 的类型。
        /// </summary>
        public static readonly Type IEnumerator = typeof(System.Collections.IEnumerator);
        /// <summary>
        /// 表示 <see cref="Collections.IEnumerable"/> 的类型。
        /// </summary>
        public static readonly Type IEnumerable = typeof(System.Collections.IEnumerable);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.IEnumerable&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type IGEnumerable = typeof(System.Collections.Generic.IEnumerable<>);
        /// <summary>
        /// 表示 <see cref="Collections.Hashtable"/> 的类型。
        /// </summary>
        public static readonly Type Hashtable = typeof(System.Collections.Hashtable);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.HashSet&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GHashSet = typeof(System.Collections.Generic.HashSet<>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.Queue&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GQueue = typeof(System.Collections.Generic.Queue<>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.Stack&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GStack = typeof(System.Collections.Generic.Stack<>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type HybridDictionary = typeof(System.Collections.Specialized.HybridDictionary);
        /// <summary>
        /// 表示 <see cref="Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GConcurrentDictionary = typeof(System.Collections.Concurrent.ConcurrentDictionary<,>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GDictionary = typeof(System.Collections.Generic.Dictionary<,>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GIDictionary = typeof(System.Collections.Generic.IDictionary<,>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.IList&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GIList = typeof(System.Collections.Generic.IList<>);
        /// <summary>
        /// 表示 <see cref="Collections.Generic.List&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GList = typeof(System.Collections.Generic.List<>);
        /// <summary>
        /// 表示 <see cref="Collections.ICollection"/> 的类型。
        /// </summary>
        public static readonly Type ICollection = typeof(System.Collections.ICollection);
        /// <summary>
        /// 表示 <see cref="Collections.ObjectModel.Collection&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GCollection = typeof(System.Collections.ObjectModel.Collection<>);
        /// <summary>
        /// 表示 <see cref="Collections.IDictionary"/> 的类型。
        /// </summary>
        public static readonly Type IDictionary = typeof(System.Collections.IDictionary);
        /// <summary>
        /// 表示 <see cref="Collections.IList"/> 的类型。
        /// </summary>
        public static readonly Type IList = typeof(System.Collections.IList);

        #endregion

        #region 数据类型

        /// <summary>
        /// 表示 <see cref="DataSet"/> 的类型。
        /// </summary>
        public static readonly Type DataSet = typeof(System.Data.DataSet);
        /// <summary>
        /// 表示 <see cref="DataTable"/> 的类型。
        /// </summary>
        public static readonly Type DataTable = typeof(System.Data.DataTable);
        /// <summary>
        /// 表示 <see cref="DataRow"/> 的类型。
        /// </summary>
        public static readonly Type DataRow = typeof(System.Data.DataRow);
        /// <summary>
        /// 表示 <see cref="DataColumn"/> 的类型。
        /// </summary>
        public static readonly Type DataColumn = typeof(System.Data.DataColumn);

        #endregion

        #region 基础类型

        /// <summary>
        /// 表示 <see cref="Object"/> 的类型。
        /// </summary>
        public static readonly Type Object = typeof(System.Object);
        /// <summary>
        /// 表示 <see cref="Object"/> 数组的类型。
        /// </summary>
        public static readonly Type ObjectArray = typeof(System.Object[]);
        /// <summary>
        /// 表示 <see cref="Object"/> 的类型（ref）。
        /// </summary>
        public static readonly Type RefObject = Object.MakeByRefType();
        /// <summary>
        /// 表示 <see cref="String"/> 的类型。
        /// </summary>
        public static readonly Type String = typeof(System.String);
        /// <summary>
        /// 表示 <see cref="Text.StringBuilder"/> 的类型。
        /// </summary>
        public static readonly Type StringBuilder = typeof(System.Text.StringBuilder);
        /// <summary>
        /// 表示 <see cref="DBNull"/> 的类型。
        /// </summary>
        public static readonly Type DBNull = typeof(DBNull);
        /// <summary>
        /// 表示 <see cref="Byte"/> 数组的类型。
        /// </summary>
        public static readonly Type ByteArray = typeof(System.Byte[]);
        /// <summary>
        /// 表示 <see cref="Char"/> 数组数组的类型。
        /// </summary>
        public static readonly Type CharArray = typeof(System.Char[]);

        #region 值类型

        /// <summary>
        /// 表示 <see cref="Boolean"/> 的类型。
        /// </summary>
        public static readonly Type Boolean = typeof(System.Boolean);
        /// <summary>
        /// 表示 <see cref="Byte"/> 的类型。
        /// </summary>
        public static readonly Type Byte = typeof(System.Byte);
        /// <summary>
        /// 表示 <see cref="Char"/> 的类型。
        /// </summary>
        public static readonly Type Char = typeof(System.Char);
        /// <summary>
        /// 表示 <see cref="DateTime"/> 的类型。
        /// </summary>
        public static readonly Type DateTime = typeof(System.DateTime);
        /// <summary>
        /// 表示 <see cref="DateTimeOffset"/> 的类型。
        /// </summary>
        public static readonly Type DateTimeOffset = typeof(System.DateTimeOffset);
        /// <summary>
        /// 表示 <see cref="Decimal"/> 的类型。
        /// </summary>
        public static readonly Type Decimal = typeof(System.Decimal);
        /// <summary>
        /// 表示 <see cref="Double"/> 的类型。
        /// </summary>
        public static readonly Type Double = typeof(System.Double);
        /// <summary>
        /// 表示 <see cref="Guid"/> 的类型。
        /// </summary>
        public static readonly Type Guid = typeof(System.Guid);
        /// <summary>
        /// 表示 <see cref="System.Data.SqlTypes.SqlGuid"/> 的类型。
        /// </summary>
        public static readonly Type SqlGuid = typeof(System.Data.SqlTypes.SqlGuid);
        /// <summary>
        /// 表示 <see cref="Int16"/> 的类型。
        /// </summary>
        public static readonly Type Int16 = typeof(System.Int16);
        /// <summary>
        /// 表示 <see cref="Int32"/> 的类型。
        /// </summary>
        public static readonly Type Int32 = typeof(System.Int32);
        /// <summary>
        /// 表示 <see cref="Int64"/> 的类型。
        /// </summary>
        public static readonly Type Int64 = typeof(System.Int64);
        /// <summary>
        /// 表示 <see cref="SByte"/> 的类型。
        /// </summary>
        public static readonly Type SByte = typeof(System.SByte);
        /// <summary>
        /// 表示 <see cref="Single"/> 的类型。
        /// </summary>
        public static readonly Type Single = typeof(System.Single);
        /// <summary>
        /// 表示 <see cref="TimeSpan"/> 的类型。
        /// </summary>
        public static readonly Type TimeSpan = typeof(System.TimeSpan);
        /// <summary>
        /// 表示 <see cref="UInt16"/> 的类型。
        /// </summary>
        public static readonly Type UInt16 = typeof(System.UInt16);
        /// <summary>
        /// 表示 <see cref="UInt32"/> 的类型。
        /// </summary>
        public static readonly Type UInt32 = typeof(System.UInt32);
        /// <summary>
        /// 表示 <see cref="UInt64"/> 的类型。
        /// </summary>
        public static readonly Type UInt64 = typeof(System.UInt64);

        #endregion

        #endregion

        /// <summary>
        /// 表示 True 的字符串形式。
        /// </summary>
        public static readonly string[] TrueStrings = { "true", "是", "校验", "checked", "1", "yes", "selected", "ok" };
        /// <summary>
        /// 表示 Flase 的字符串形式。
        /// </summary>
        public static readonly string[] FlaseStrings = { "flase", "否", "非校验", "unchecked", "0", "no", "unselected", "cancel", string.Empty };

        /// <summary>
        /// 表示浮点数的数据类型集合。
        /// </summary>
        public static readonly Type[] NumberFloatTypes = { Single, Double, Decimal };

        /// <summary>
        /// 表示数字的数据类型集合。
        /// </summary>
        public static readonly Type[] NumberTypes = {SByte,Byte, UInt16, UInt32, UInt64
                                                     ,Int16, Int32, Int64
                                                     , Single,Double,Decimal };
    }
}
