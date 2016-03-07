using System;

namespace Aoite.Data
{
    /// <summary>
    /// 定义数据源查询与交互引擎的方法。
    /// </summary>
    public interface IDbEngine
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的提供程序。
        /// </summary>
        IDbEngineProvider Provider { get; }
        /// <summary>
        /// 获取当前上下文所属的交互引擎。
        /// </summary>
        DbEngine Owner { get; }

        /// <summary>
        /// 创建并返回一个 <see cref="IDbContext"/>。返回当前线程上下文包含的 <see cref="IDbContext"/> 或创建一个新的  <see cref="IDbContext"/>。
        /// <para>当释放一个 <see cref="IDbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        IDbContext Context { get; }
        /// <summary>
        /// 创建并返回一个事务性 <see cref="IDbContext"/>。返回当前线程上下文包含的 <see cref="IDbContext"/> 或创建一个新的  <see cref="IDbContext"/>。
        /// <para>当释放一个 <see cref="IDbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        IDbContext ContextTransaction { get; }
        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        IDbExecutor Execute(ExecuteCommand command);
#if !NET45 && !NET40
        /// <summary>
        /// 执行指定的命令。
        /// </summary>
        /// <param name="fs">一个复合格式字符串</param>
        /// <returns>数据源查询与交互的执行器。</returns>
        IDbExecutor Execute(FormattableString fs);
#endif
    }
}
