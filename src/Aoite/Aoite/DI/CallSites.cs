using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Aoite.DI
{
    /// <summary>
    /// 定义一个服务调用位置。
    /// </summary>
    public interface ICallSite
    {
        /// <summary>
        /// 获取服务的生命周期。
        /// </summary>
        ServiceLifetime Lifetime { get; }
        /// <summary>
        /// 调用服务。
        /// </summary>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns>返回一个服务的实例。</returns>
        object Invoke(params object[] lastMappingValues);
    }

    abstract class CallSiteBase : ICallSite
    {
        public abstract ServiceLifetime Lifetime { get; }

        public object Invoke(params object[] lastMappingValues)
        {
            var obj = this.OnInvoke(lastMappingValues);
            return obj;
        }

        protected abstract object OnInvoke(params object[] lastMappingValues);
    }

    class TransientCallSite : CallSiteBase
    {
        public override ServiceLifetime Lifetime { get { return ServiceLifetime.Transient; } }

        private readonly Lazy<InstanceCreatorCallback> _lazyCallback;

        public TransientCallSite(Func<InstanceCreatorCallback> callbackFactory)
        {
            this._lazyCallback = new Lazy<InstanceCreatorCallback>(callbackFactory);
        }

        protected override object OnInvoke(params object[] lastMappingValues) => this._lazyCallback.Value(lastMappingValues);
    }
    class ScopedCallSite : CallSiteBase
    {
        public override ServiceLifetime Lifetime { get { return ServiceLifetime.Scoped; } }
        private ThreadLocal<object> _local;
        private readonly ThreadLocal<InstanceCreatorCallback> _lazyCallback;
        private readonly string Id = Guid.NewGuid().ToString();

        public ScopedCallSite(Func<InstanceCreatorCallback> callbackFactory)
        {
            this._lazyCallback = new ThreadLocal<InstanceCreatorCallback>(callbackFactory);
        }

        protected override object OnInvoke(params object[] lastMappingValues)
        {
            if(GA.IsWebRuntime) return Webx.GetTemp(Id, () => this._lazyCallback.Value(lastMappingValues));

            if(this._local == null) this._local = new ThreadLocal<object>();
            if(!this._local.IsValueCreated) this._local.Value = this._lazyCallback.Value(lastMappingValues);
            return this._local.Value;
        }
    }
    class SingletonCallSite : CallSiteBase
    {
        public override ServiceLifetime Lifetime { get { return ServiceLifetime.Singleton; } }
        private readonly Lazy<object> _lazyInstance;

        public SingletonCallSite(Func<InstanceCreatorCallback> callbackFactory)
        {
            this._lazyInstance = new Lazy<object>(() => callbackFactory()());
        }

        protected override object OnInvoke(params object[] lastMappingValues)
        {
            return this._lazyInstance.Value;
        }
    }

    class LastMappingCallSite : CallSiteBase
    {
        public override ServiceLifetime Lifetime { get { return ServiceLifetime.LastMapping; } }

        private ParameterInfo _parameter;
        private Type _actualType;
        private int _valueIndex;
        public LastMappingCallSite(Type actualType, ParameterInfo parameter, int valueIndex)
        {
            this._actualType = actualType;
            this._parameter = parameter;
            this._valueIndex = valueIndex;
        }

        protected override object OnInvoke(params object[] lastMappingValues)
        {
            if(lastMappingValues == null || lastMappingValues.Length <= this._valueIndex)
                throw new ArgumentException(this._actualType.FullName + "：构造函数的参数“" + this._parameter.Name + "”指定了后期映射关系，但调用方却没有传递映射值！", this._parameter.Name);

            return lastMappingValues[this._valueIndex];
        }
    }
}
