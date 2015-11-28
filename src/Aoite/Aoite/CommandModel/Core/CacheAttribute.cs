using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型执行器具有缓存的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CacheAttribute : Attribute, IExecutorAttribute
    {
        private string _Group;
        /// <summary>
        /// 设置或获取缓存的分组。
        /// </summary>
        public string Group { get { return _Group; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CacheAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="group">缓存的分组。</param>
        public CacheAttribute(string group)
        {
            if(string.IsNullOrEmpty(group)) throw new ArgumentNullException("group");
            this._Group = group;
        }

        bool ICommandHandler<ICommand>.RaiseExecuting(IContext context, ICommand command)
        {
            var commandCache = command as ICommandCache;
            if(commandCache == null) throw new NotSupportedException(command.GetType().FullName + "：命令模型没有实现缓存接口。");
            var strategy = commandCache.CreateStrategy(context);
            if(strategy == null || string.IsNullOrEmpty(strategy.Key))
                throw new NotSupportedException(command.GetType().FullName + "：命令模型返回了无效的策略信息。");

            var value = strategy.GetCache(this._Group);
            if(value == null) return true;
            return !commandCache.SetCacheValue(value);
        }

        void ICommandHandler<ICommand>.RaiseExecuted(IContext context, ICommand command, Exception exception)
        {
            if(exception != null) return;
            var commandCache = command as ICommandCache;
            commandCache.CreateStrategy(context).SetCache(this._Group, commandCache.GetCacheValue());
        }
    }
}
