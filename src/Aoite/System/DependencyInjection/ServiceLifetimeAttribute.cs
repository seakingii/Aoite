using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.DI;

namespace System
{
    public class ServiceLifetimeAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public ServiceLifetimeAttribute(ServiceLifetime lifetime)
        {
            switch(lifetime)
            {
                case ServiceLifetime.Transient:
                case ServiceLifetime.Scoped:
                case ServiceLifetime.Singleton:
                    break;
                default:
                    throw new NotSupportedException($"不支持服务生命周期类型{lifetime}。");
            }

            this.Lifetime = lifetime;
        }
    }
}
