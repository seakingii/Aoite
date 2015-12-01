using System;

namespace Aoite.Redis.Commands
{
    /// <summary>
    /// 表示一个 Redis 的命令。
    /// </summary>
    public abstract class RedisCommand
    {
        private readonly string _command;
        private readonly object[] _args;
        /// <summary>
        /// 获取命令。
        /// </summary>
        public string Command { get { return _command; } }
        /// <summary>
        /// 获取参数。
        /// </summary>
        public object[] Arguments { get { return _args; } }

        internal RedisCommand(string command, params object[] args)
        {
            if(string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if(args == null) throw new ArgumentNullException(nameof(args));
            _command = command;
            _args = args;
        }

        internal abstract void SetCallback<TValue>(Action<TValue> callback);
        internal abstract void RunCallback(object value);
        internal abstract object ObjectParse(RedisExecutor executor);
        /*
        /// <summary>
        /// 表示提供一个命令的创建功能。
        /// </summary>
        public static class Commands
        {
            /// <summary>
            /// 创建一个 Redis 数组的命令。
            /// </summary>
            /// <typeparam name="T">返回值的数据类型。</typeparam>
            /// <param name="command">命令。</param>
            /// <param name="parseItemCount">返回值的解析项量。</param>
            /// <returns>返回一个命令。</returns>
            public static RedisCommand<T[]> Array<T>(RedisCommand<T> command, int parseItemCount = 1)
            {
                return RedisArray.Create(command, parseItemCount);

            }

            public static RedisCommand<bool> Boolean(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisBoolean(command, args);
            }
            public static RedisCommand<DateTime> Date(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisDate(command, args);
            }
            public static RedisCommand<double> Float(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisFloat(command, args);
            }
            public static RedisCommand<long> Integer(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisInteger(command, args);
            }
            public static RedisCommand<T> Item<T>(bool checkType, string command, params object[] args)
                where T : IRedisItem
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisItem<T>(checkType, command, args);
            }

            public static RedisCommand<RedisType> KeyType(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisKeyType(command, args);
            }
            public static RedisCommand<object> Object(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisObject(command, args);
            }
            public static RedisCommand<object> Object(string command, params object args)
            {
                if(command == null) throw new ArgumentNullException(nameof(command));
                return new RedisObject(command, args);
            }

        }*/
    }
}
