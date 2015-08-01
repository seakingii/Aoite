using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 定义一个 Redis 客户端。
    /// </summary>
    public interface IRedisClient : IObjectDisposable
    {
        /// <summary>
        /// 获取当前 Redis 的在应用程序域中的唯一编号。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        long Id { get; }
        /// <summary>
        /// 执行指定的 Redis 命令。
        /// </summary>
        /// <typeparam name="T">命令返回值的数据类型。</typeparam>
        /// <param name="command">Redis 命令。</param>
        /// <returns>返回执行后的返回值。</returns>
        T Execute<T>(RedisCommand<T> command);
        /// <summary>
        /// 开始一个新的事务。
        /// </summary>
        /// <returns>如果事务已存在，将会抛出一个错误，否则返回一个新的事务。</returns>
        IRedisTransaction BeginTransaction();
    }

}
