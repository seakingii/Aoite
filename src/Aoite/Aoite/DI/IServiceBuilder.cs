using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.DI
{
    /// <summary>
    /// 定义服务的构建器。
    /// </summary>
    public interface IServiceBuilder
    {
        /// <summary>
        /// 添加或覆盖一个预期服务类型。
        /// </summary>
        /// <param name="expectType">预期服务类型。</param>
        /// <returns>类型服务的绑定器。</returns>
        ITypeServiceBinder Use(Type expectType);

        /// <summary>
        /// 添加一个预期服务类型。
        /// </summary>
        /// <param name="expectType">预期服务类型。</param>
        /// <returns>类型服务的绑定器。</returns>
        ITypeServiceBinder UseRange(Type expectType);

        /// <summary>
        /// 添加或覆盖一个值服务。
        /// </summary>
        /// <param name="name">值服务的参数名称。</param>
        /// <returns>值型服务的绑定器。</returns>
        IValueServiceBinder Use(string name);

        /// <summary>
        /// 添加或覆盖一个值服务。
        /// </summary>
        /// <param name="expectType">预期服务类型。</param>
        /// <param name="name">值服务的参数名称。</param>
        /// <returns>值型服务的绑定器。</returns>
        IValueServiceBinder Use(Type expectType, string name);

        /// <summary>
        /// 将当前构建器的所有服务提升到父服务。
        /// </summary>
        void Promote();
    }


    class ServiceBuilder : ObjectDisposableBase, IServiceBuilder
    {
        private IocContainer _locator;
        public readonly Lazy<List<TypeServiceBinder>> TypeBinders;
        public readonly Lazy<List<ValueServiceBinder>> ValueBinders;
        private bool _promote;
        public bool IsPromote => _promote;

        public ServiceBuilder(IocContainer locator)
        {
            if(locator == null) throw new ArgumentNullException(nameof(locator));
            this._locator = locator;
            this.TypeBinders = new Lazy<List<TypeServiceBinder>>();
            this.ValueBinders = new Lazy<List<ValueServiceBinder>>();
        }

        private ITypeServiceBinder InnerSet(Type expectType, bool overiwrite)
        {
            if(expectType == null) throw new ArgumentNullException(nameof(expectType));

            var binder = new TypeServiceBinder(this._locator, this, expectType, overiwrite);
            this.TypeBinders.Value.Add(binder);
            return binder;
        }

        public ITypeServiceBinder UseRange(Type expectType) => this.InnerSet(expectType, false);

        public ITypeServiceBinder Use(Type expectType) => this.InnerSet(expectType, true);

        public void Promote() => this._promote = true;

        private IValueServiceBinder InnserSet(Type expectType, string name, bool checkexpectTypeNull)
        {
            if(checkexpectTypeNull && expectType == null) throw new ArgumentNullException(nameof(expectType));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var binder = new ValueServiceBinder(this._locator, this, expectType, name);
            this.ValueBinders.Value.Add(binder);
            return binder;
        }

        public IValueServiceBinder Use(string name) => this.InnserSet(null, name, false);

        public IValueServiceBinder Use(Type expectType, string name) => this.InnserSet(expectType, name, true);
    }
}
