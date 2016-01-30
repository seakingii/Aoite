using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 一个依赖注入的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class DIAttribute : CustomModelBinderAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="DIAttribute"/> 类的新实例。
        /// </summary>
        public DIAttribute() { }
     
        static readonly IModelBinder Binder = new DIModelBinder();
        /// <summary>
        /// Retrieves the associated model binder.
        /// </summary>
        /// <returns>A reference to an object that implements the System.Web.Mvc.IModelBinder interface.</returns>
        public override IModelBinder GetBinder() => Binder;
    }
}
