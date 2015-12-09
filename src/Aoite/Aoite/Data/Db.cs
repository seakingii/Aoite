using Aoite.Data;
using System.Configuration;

namespace System
{
    /// <summary>
    /// 有关于数据的上下文。
    /// </summary>
    public static partial class Db
    {
        private readonly static object SyncObjectE = new object();

        const string DEFAULT_CONNECTION_NAME = "DB:";

        private static bool _isInitialized = false;
        private static void Initialize()
        {
            if(_isInitialized) return;
            lock (SyncObjectE)
            {
                if(_isInitialized) return;

                if(_Engine == null)
                {
                    foreach(ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
                    {
                        string name = conn.Name;
                        if(string.IsNullOrWhiteSpace(name) || !name.iStartsWith(DEFAULT_CONNECTION_NAME)) continue;

                        string connectionString = conn.ConnectionString;
                        name = name.Remove(0, DEFAULT_CONNECTION_NAME.Length);

                        var engine = DbEngine.Create(conn.ProviderName, connectionString);
                        if(engine == null) throw new SettingsPropertyWrongTypeException("无效的数据源提供程序。发生在“" + conn.Name + "”的 providerName 属性值为“" + conn.ProviderName + "”。");
                        _Engine = engine;
                        break;
                    }
                }

                _isInitialized = true;
            }
        }


        private static DbEngine _Engine;
        /// <summary>
        /// 获取当前运行环境的数据源查询与交互引擎的实例。
        /// </summary>
        public static DbEngine Engine
        {
            get
            {
                Initialize();
                return _Engine;
            }
        }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public static bool IsThreadContext
        {
            get
            {
                if(Engine == null) return false;
                return Engine.IsThreadContext;
            }
        }

        /// <summary>
        /// 创建并返回一个 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public static IDbContext Context
        {
            get
            {
                return Engine?.Context;
            }
        }

        /// <summary>
        /// 创建并返回一个事务性 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public static IDbContext ContextTransaction
        {
            get
            {
                return Engine?.ContextTransaction;
            }
        }

        /// <summary>
        /// 释放并关闭当前线程上下文的 <see cref="Db.Context"/>。
        /// </summary>
        public static void ResetContext()
        {
            var engine = Engine;
            if(engine == null) return;
            engine.ResetContext();
        }

        /// <summary>
        /// 设置当前运行环境的数据源查询与交互引擎的实例。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        public static void SetEngine(DbEngine engine)
        {
            lock(SyncObjectE)
            {
                _Engine = engine;
            }
        }

    }
}
