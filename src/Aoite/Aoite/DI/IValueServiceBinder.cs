using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.DI
{
    /// <summary>
    /// 定义一个值服务的绑定器。
    /// </summary>
    public interface IValueServiceBinder: IServiceBinder
    {
        /// <summary>
        /// 获取值服务的参数名称。
        /// </summary>
        string Name { get; }
    }

    class ValueServiceBinder : ServiceBinderBase, IValueServiceBinder
    {
        private readonly string _name;

        public string Name => this._name;

        public ValueServiceBinder(IocContainer locator, ServiceBuilder builder, Type expectType, string name)
            : base(locator, builder, expectType)
        {
            this._name = name;
        }

    }
}
