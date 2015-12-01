using System;

namespace Aoite.Redis.Commands
{
    /// <summary>
    /// 表示一个 Redis 的命令。
    /// </summary>
    /// <typeparam name="T">命令返回值的数据类型。</typeparam>
    public abstract class RedisCommand<T> : RedisCommand
    {
        internal RedisCommand(string command, params object[] args) : base(command, args) { }

        internal abstract T Parse(RedisExecutor executor);

        Action<T> _callback;
        internal override void SetCallback<TValue>(Action<TValue> callback)
        {
            _callback = callback as Action<T>;
        }

        internal override void RunCallback(object value)
        {
            var callback = this._callback;
            if(callback != null)
            {
                callback((T)value);
                this._callback = null;
            }
        }
        internal override object ObjectParse(RedisExecutor executor)
        {
            return Parse(executor);
        }
    }
}
