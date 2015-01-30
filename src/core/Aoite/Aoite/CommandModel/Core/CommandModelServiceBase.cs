using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的基础服务。
    /// </summary>
    public abstract class CommandModelServiceBase : ObjectDisposableBase, ICommandModelService
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CommandModelServiceBase"/> 类的新实例。
        /// </summary>
        public CommandModelServiceBase() { }

        /// <summary>
        /// 指定服务容器，初始化一个 <see cref="Aoite.CommandModel.CommandModelServiceBase"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandModelServiceBase(IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException("container");
            this._Container = container;
        }

        private IIocContainer _Container;
        /// <summary>
        /// 设置或获取命令模型服务容器。
        /// </summary>
        public IIocContainer Container
        {
            get { return this._Container; }
            set
            {
                this._Container = value;
                if(this._Container != null) this._Container.AddService<ICommandModelService>(this);
            }
        }

        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public virtual dynamic User { get { return this._Container.GetUser(); } }

        /// <summary>
        /// 获取命令总线。
        /// </summary>
        protected ICommandBus Bus { get { return this._Container.GetService<ICommandBus>(); } }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>返回命令模型。</returns>
        protected virtual TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            return this.Bus.Execute(command, executing, executed);
        }

        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>返回一个异步操作。</returns>
        protected virtual Task<TCommand> ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            return this.Bus.ExecuteAsync(command, executing, executed);
        }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        protected IDisposable AcquireLock<T>(TimeSpan? timeout = null)
        {
            return this.AcquireLock(typeof(T).FullName, timeout);
        }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <param name="key">锁的键。</param>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        protected virtual IDisposable AcquireLock(string key, TimeSpan? timeout = null)
        {
            return this._Container.GetService<ILockProvider>().Lock(key, timeout);
        }

        /// <summary>
        /// 获取指定数据类型键的原子递增序列。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected long Increment<T>(long increment = 1)
        {
            return this.Increment(typeof(T).FullName, increment);
        }

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected virtual long Increment(string key, long increment = 1)
        {
            return this._Container.GetService<ICounterProvider>().Increment(key, increment);
        }

        /// <summary>
        /// 开始事务模式。
        /// </summary>
        /// <returns>返回一个事务。</returns>
        protected virtual ITransaction BeginTransaction()
        {
            return new DefaultTransaction();
        }
    }
}
