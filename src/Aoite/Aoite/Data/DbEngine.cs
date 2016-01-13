using System;
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎。
    /// </summary>
    public class DbEngine : IDbEngine
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的提供程序。
        /// </summary>
        public IDbEngineProvider Provider { get; }

        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        public DbEngine Owner { get { return this; } }

        /// <summary>
        /// 给定数据源查询与交互引擎的提供程序，初始化一个 <see cref="DbEngine"/> 类的新实例。
        /// </summary>
        /// <param name="provider">数据源查询与交互引擎的提供程序。</param>
        public DbEngine(IDbEngineProvider provider)
        {
            if(provider == null) throw new ArgumentNullException(nameof(provider));

            this.Provider = provider;
        }

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IDbExecutor Execute(ExecuteCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            return new DbExecutor(this, command, null, null, true);
        }

        private readonly System.Threading.ThreadLocal<DbContext> _threadLocalContent = new System.Threading.ThreadLocal<DbContext>();
        /// <summary>
        /// 释放并关闭当前线程上下文的 <see cref="IDbContext"/>。
        /// </summary>
        public void ResetContext()
        {
            var context = this._threadLocalContent.Value;

            if(context != null)
            {
                context.Dispose();
                this._threadLocalContent.Value = null;
            }
        }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public bool IsThreadContext { get { return this._threadLocalContent.Value != null; } }

        /// <summary>
        /// 创建并返回一个 <see cref="IDbContext"/>。返回当前线程上下文包含的 <see cref="IDbContext"/> 或创建一个新的  <see cref="IDbContext"/>。
        /// <para>当释放一个 <see cref="IDbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual IDbContext Context
        {
            get
            {
                if(this._threadLocalContent.Value == null) this._threadLocalContent.Value = new DbContext(this);
                return this._threadLocalContent.Value;
            }
        }

        /// <summary>
        /// 创建并返回一个事务性 <see cref="IDbContext"/>。返回当前线程上下文包含的 <see cref="IDbContext"/> 或创建一个新的  <see cref="IDbContext"/>。
        /// <para>当释放一个 <see cref="IDbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual IDbContext ContextTransaction { get { return this.Context.OpenTransaction(); } }


        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public event ExecutingEventHandler Executing;
        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public event ExecutedEventHandler Executed;

        /// <summary>
        /// 表示 <see cref="Executing"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="command">执行的命令。</param>
        /// <param name="dbCommand">执行的 <see cref="DbCommand"/>。</param>
        internal protected virtual void OnExecuting(IDbEngine engine, ExecuteType type, ExecuteCommand command, DbCommand dbCommand)
            => this.Executing?.Invoke(engine, command.GetEventArgs(type, dbCommand, null));

        /// <summary>
        /// 表示 <see cref="Executed"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="result">操作的返回值。</param>
        /// <param name="command">执行的命令。</param>
        /// <param name="dbCommand">执行的 <see cref="DbCommand"/>。</param>
        internal protected virtual void OnExecuted(IDbEngine engine, ExecuteType type, ExecuteCommand command, DbCommand dbCommand, object result)
            => this.Executed?.Invoke(engine, command.GetEventArgs(type, dbCommand, result));


        private const string Provider_Microsoft_SQL_Server_Simple = "sql";
        private const string Provider_Microsoft_SQL_Server = "mssql";
        private const string Provider_Microsoft_SQL_Server_Compact_Simple1 = "sqlce";
        private const string Provider_Microsoft_SQL_Server_Compact_Simple2 = "ce";
        private const string Provider_Microsoft_SQL_Server_Compact = "mssqlce";
        private const string Provider_Microsoft_OleDb2003 = "oledb2003";
        private const string Provider_Microsoft_OleDb2007 = "oledb2007";
        private const string Provider_SQLite = "sqlite";
        private const string Provider_Oracle = "oracle";
        private const string Provider_MySql = "mysql";

        private static readonly Type DbProvidersAttributeType = typeof(DbProvidersAttribute);

        private static readonly ConcurrentDictionary<string, Type> RegisterProviders = new ConcurrentDictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);

        private static Type GetProviderType(string provider)
        {
            var types = from g in ObjectFactory.AllTypes
                        from type in g.Value
                        where type.IsDefined(DbProvidersAttributeType, false)
                        select type;
            return (from type in types
                    where type.GetAttribute<DbProvidersAttribute>().Names.Contains(provider, StringComparer.CurrentCultureIgnoreCase)
                    select type).FirstOrDefault();

        }

        private static IDbEngineProvider CreateProvider(string provider, string connectionString)
        {
            var type = RegisterProviders.GetOrAdd(provider, GetProviderType);
            if(type == null)
            {
                RegisterProviders.TryRemove(provider, out type);
                throw new NotSupportedException($"当前运行环境找不到名称为“{provider}”的数据库提供程序。");
            }

            var ctor = type.GetConstructor(new Type[] { Types.String });
            if(ctor == null) throw new NotSupportedException($"当前运行环境名称为“{provider}”的数据库提供程序找不到“{type.FullName}(connectionString)” 的构造函数。");
            return (IDbEngineProvider)DynamicFactory.CreateConstructorHandler(ctor)(connectionString);
            //- 支持.NET 4.0 4.5 4.6
        }

        /// <summary>
        /// 根据指定的提供程序创建一个数据源查询与交互引擎的实例。
        /// </summary>
        /// <param name="provider">提供程序。</param>
        /// <param name="connectionString">连接字符串。</param>
        public static DbEngine Create(string provider, string connectionString)
        {
            if(string.IsNullOrWhiteSpace(provider)) throw new ArgumentNullException(nameof(provider));
            if(string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var engineProvider = CreateProvider(provider, connectionString);
            if(engineProvider == null) throw new ArgumentException($"非法的数据源提供程序“{provider}”。", nameof(provider));
            return new DbEngine(engineProvider);
        }

#if !NET45 && !NET40

        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="fs">一个复合格式字符串</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        public IDbExecutor Execute(FormattableString fs) => this.Execute(this.Parse(fs));

        /// <summary>
        /// 将一个复合格式字符串转换为 <see cref="ExecuteCommand"/> 的对象实例。
        /// </summary>
        /// <param name="fs">一个复合格式字符串</param>
        /// <returns><see cref="ExecuteCommand"/> 的对象实例。</returns>
        public ExecuteCommand Parse(FormattableString fs)
        {
            if(fs == null) throw new ArgumentNullException(nameof(fs));

            var parameters = new ExecuteParameterCollection();
            var sfp = new SqlFormatProvider(parameters, this.Provider);
            var text = fs.ToString(sfp);
            return new ExecuteCommand(text, parameters);
        }

        class SqlFormatProvider : IFormatProvider
        {
            private readonly SqlFormatter _formatter;

            public SqlFormatProvider(ExecuteParameterCollection parameters, IDbEngineProvider engineProvider)
            {
                _formatter = new SqlFormatter(parameters, engineProvider);
            }

            public object GetFormat(Type formatType)
            {
                if(formatType == typeof(ICustomFormatter)) return _formatter;
                return null;
            }

            class SqlFormatter : ICustomFormatter
            {
                ExecuteParameterCollection _parameters;
                IDbEngineProvider _engineProvider;
                public SqlFormatter(ExecuteParameterCollection parameters, IDbEngineProvider engineProvider)
                {
                    _parameters = parameters;
                    _engineProvider = engineProvider;
                }
                private int index = 0;
                public string Format(string format, object arg, IFormatProvider formatProvider)
                {
                    if(format == ":") return Convert.ToString(arg);
                    var name = "p" + index++;
                    _parameters.Add(_engineProvider.EscapeName(name, NamePoint.Parameter), arg);
                    return _engineProvider.EscapeName(name, NamePoint.Value);
                }
            }
        }
#endif
    }
}
