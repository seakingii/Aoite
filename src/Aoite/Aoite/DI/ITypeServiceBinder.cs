using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.DI
{
    /// <summary>
    /// 定义类型服务的绑定器。
    /// </summary>
    public interface ITypeServiceBinder : IServiceBinder
    {
        /// <summary>
        /// 获取一个值，指示当前绑定器是否覆盖旧的绑定。
        /// </summary>
        bool Overwrite { get; }
        /// <summary>
        /// 绑定为短暂模式的服务。
        /// </summary>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Transient();
        /// <summary>
        /// 绑定为短暂模式的服务。
        /// </summary>
        /// <param name="actualType">实际的服务类型。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Transient(Type actualType);

        /// <summary>
        /// 绑定为单例模式的服务。
        /// </summary>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Singleton();
        /// <summary>
        /// 绑定为单例模式的服务。
        /// </summary>
        /// <param name="actualType">实际的服务类型。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Singleton(Type actualType);

        /// <summary>
        /// 绑定为范围模式的服务。
        /// </summary>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Scoped();
        /// <summary>
        /// 绑定为范围模式的服务。
        /// </summary>
        /// <param name="actualType">实际的服务类型。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder Scoped(Type actualType);

        /// <summary>
        /// 绑定为智能模式的服务。根据 <see cref="IServiceBinder.ExpectType"/> 的特性创建不同模式的服务（默认为短暂模式）。
        /// </summary>
        /// <returns>服务构建器。</returns>
        IServiceBuilder As();
        /// <summary>
        /// 绑定为智能模式的服务。根据 <see cref="IServiceBinder.ExpectType"/> 的特性创建不同模式的服务（默认为短暂模式）。
        /// </summary>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder As(InstanceCreatorCallback callback);
        /// <summary>
        /// 绑定为智能模式的服务。根据 <see cref="IServiceBinder.ExpectType"/> 的特性创建不同模式的服务（默认为短暂模式）。
        /// </summary>
        /// <param name="actualType">实际的服务类型。</param>
        /// <returns>服务构建器。</returns>
        IServiceBuilder As(Type actualType);
    }

    class TypeServiceBinder : ServiceBinderBase, ITypeServiceBinder
    {
        private readonly bool _overwrite;
        public bool Overwrite => this._overwrite;

        public TypeServiceBinder(IocContainer locator, ServiceBuilder builder, Type expectType, bool overwrite)
            : base(locator, builder, expectType)
        {
            this._overwrite = overwrite;
        }

        private TypeServiceBinder ThrowIfDAR()
        {
            if(this._locator.DisabledAutoResolving) throw new NotSupportedException($"定位器禁止了智能解析，请显示注册{this._expectType.FullName}目标类型。");
            return this;
        }

        private InstanceCreatorCallback CreateCallback()
        {
            return this.CreateCallback(this._locator.ForceFindActualType(this._expectType));
        }

        private InstanceCreatorCallback CreateCallback(Type actualType)
        {
            return this._locator.CreateCallback(this._expectType, actualType);
        }

        private TypeServiceBinder TestActualType(Type actualType)
        {
            this._locator.TestActualType(actualType);
            return this;
        }

        public IServiceBuilder As()
            => this.ThrowIfDAR().SetCallSite(this._locator.CreateFromActualType(this._expectType, null));

        public IServiceBuilder As(Type actualType)
            => this.ThrowIfDAR().TestActualType(actualType).SetCallSite(this._locator.CreateFromActualType(this._expectType, actualType));

        public IServiceBuilder Scoped()
            => this.ThrowIfDAR().SetCallSite(new ScopedCallSite(this.CreateCallback));

        public IServiceBuilder Scoped(Type actualType)
            => this.TestActualType(actualType).SetCallSite(new ScopedCallSite(() => this.CreateCallback(actualType)));

        public IServiceBuilder Singleton()
            => this.ThrowIfDAR().SetCallSite(new SingletonCallSite(this.CreateCallback));

        public IServiceBuilder Singleton(Type actualType)
            => this.TestActualType(actualType).SetCallSite(new SingletonCallSite(() => this.CreateCallback(actualType)));

        public IServiceBuilder Transient()
            => this.ThrowIfDAR().SetCallSite(new TransientCallSite(this.CreateCallback));
        public IServiceBuilder Transient(Type actualType)
            => this.TestActualType(actualType).SetCallSite(new TransientCallSite(() => this.CreateCallback(actualType)));
    }
}
