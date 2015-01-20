using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 事务。
    /// <para>事务在执行 EXEC 之前，入队的命令可能会出错。比如说，命令可能会产生语法错误（参数数量错误，参数名错误，等等），或者其他更严重的错误，比如内存不足（如果服务器使用 maxmemory 设置了最大内存限制的话）。</para>
    /// <para>命令可能在 EXEC 调用之后失败。举个例子，事务中的命令可能处理了错误类型的键，比如将列表命令用在了字符串键上面，诸如此类。</para>
    /// </summary>
    public interface IRedisTransaction : IRedisClient
    {
        /// <summary>
        /// 指示当事务成功执行后，需要获取返回值的回调的方法。
        /// </summary>
        /// <typeparam name="T">返回值的数据类型。</typeparam>
        /// <param name="executor">执行的委托的返回值。</param>
        /// <param name="callback">毁掉的委托。</param>
        void On<T>(T executor, Action<T> callback);
        /// <summary>
        /// 提交当前事务。直到事务释放之前，如果没有显示提交，将会自动放弃事务。
        /// <para>提交事务抛出错误时，所有的回调方法都不会执行。但其他非失败的命令在 Redis 数据库实际已执行成功。</para>
        /// </summary>
        /// <returns>返回一个结果。</returns>
        void Commit();
    }
}
