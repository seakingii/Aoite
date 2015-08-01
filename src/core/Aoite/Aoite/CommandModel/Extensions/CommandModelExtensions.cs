using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示命令模型的扩展方法。
    /// </summary>
    public static class CommandModelExtensions
    {
        /// <summary>
        /// 注册一个事件到指定的命令模型类型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        /// <param name="event">事件。</param>
        public static void Register<TCommand>(this IEventStore eventStore, IEvent<TCommand> @event) where TCommand : ICommand
        {
            eventStore.Register(typeof(TCommand), @event);
        }

        /// <summary>
        /// 注销指定命令模型类型的一个事件。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        /// <param name="event">事件。</param>
        public static void Unregister<TCommand>(this IEventStore eventStore, IEvent<TCommand> @event) where TCommand : ICommand
        {
            eventStore.Unregister(typeof(TCommand), @event);
        }

        /// <summary>
        /// 注销指定命令模型类型的所有事件。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        public static void UnregisterAll<TCommand>(this IEventStore eventStore) where TCommand : ICommand
        {
            eventStore.UnregisterAll(typeof(TCommand));
        }

        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="provider">缓存提供程序。</param>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        public static IDisposable MultipleLock(this ILockProvider provider, params string[] keys)
        {
            return MultipleLock(provider, null, keys);
        }

        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="provider">缓存提供程序。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        public static IDisposable MultipleLock(this ILockProvider provider, TimeSpan timeout, params string[] keys)
        {
            return MultipleLock(provider, timeout, keys);
        }

        private static IDisposable MultipleLock(ILockProvider provider, TimeSpan? timeout, params string[] keys)
        {
            if(provider == null) throw new ArgumentNullException("provider");
            if(keys == null || keys.Length == 0) throw new ArgumentNullException("keys");

            Stack<IDisposable> stack = new Stack<IDisposable>(keys.Length);
            foreach(var key in keys)
            {
                stack.Push(provider.Lock(key, timeout));
            }
            return new MultipleLockKeys(stack);
        }
        class MultipleLockKeys : IDisposable
        {
            Stack<IDisposable> _stack;
            public MultipleLockKeys(Stack<IDisposable> stack)
            {
                this._stack = stack;
            }
            public void Dispose()
            {
                while(this._stack.Count > 0)
                {
                    this._stack.Pop().Dispose();
                }
            }
        }

        internal static IDbEngine GetDbEngine(this IContainerProvider provider)
        {
            return provider.Container.GetFixedService<IDbEngine>() ?? Db.Context;
        }
    }
}
