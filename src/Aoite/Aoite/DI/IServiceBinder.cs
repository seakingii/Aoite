using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.DI
{
    /// <summary>
    /// 定义服务的绑定器。
    /// </summary>
    public interface IServiceBinder
    {
        /// <summary>
        /// 获取当前绑定器的预期服务类型。
        /// </summary>
        Type ExpectType { get; }
        /// <summary>
        /// 绑定为单例模式的服务。
        /// </summary>
        /// <param name="value">要添加的服务的实例。 此对象必须实现 <see cref="ExpectType"/>> 所指示的类型或从其继承。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Singleton(object value);
        /// <summary>
        /// 绑定为单例模式的服务。
        /// </summary>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Singleton(InstanceCreatorCallback callback);
        /// <summary>
        /// 绑定为短暂模式的服务。
        /// </summary>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Transient(InstanceCreatorCallback callback);
        /// <summary>
        /// 绑定为范围模式的服务。
        /// </summary>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Scoped(InstanceCreatorCallback callback);
    }

    abstract class ServiceBinderBase : IServiceBinder
    {
        protected readonly IocContainer _container;
        protected readonly ServiceBuilder _builder;
        protected readonly Type _expectType;
        private ICallSite _callSite;
        public Type ExpectType => this._expectType;
        public ICallSite CallSite => this._callSite;

        public ServiceBinderBase(IocContainer container, ServiceBuilder builder, Type expectType)
        {
            this._container = container;
            this._builder = builder;
            this._expectType = expectType;
        }

        protected ServiceBinderBase TestCallback(InstanceCreatorCallback callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback));
            return this;
        }

        public IServiceBuilder As(InstanceCreatorCallback callback)
            => this.TestCallback(callback).SetCallSite(this._container.AutoResolving(this._expectType, null, () => callback));

        protected IServiceBuilder SetCallSite(ICallSite callSite)
        {
            if(callSite == null) throw new ArgumentNullException(nameof(callSite));
            this._callSite = callSite;
            return this._builder;
        }

        public IServiceBuilder Scoped(InstanceCreatorCallback callback)
            => this.TestCallback(callback).SetCallSite(new ScopedCallSite(() => callback));

        public IServiceBuilder Singleton(InstanceCreatorCallback callback)
            => this.TestCallback(callback).SetCallSite(new SingletonCallSite(() => callback));

        public IServiceBuilder Singleton(object value)
            => this.SetCallSite(new SingletonCallSite(() => lms => value));

        public IServiceBuilder Transient(InstanceCreatorCallback callback)
            => this.TestCallback(callback).SetCallSite(new TransientCallSite(() => callback));

    }
}
