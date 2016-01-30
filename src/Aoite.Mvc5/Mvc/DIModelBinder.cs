using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示一个依赖注入的实体绑定器。
    /// </summary>
    public class DIModelBinder : IModelBinder
    {
        private readonly IModelBinder _defaultBinder;
        internal DIModelBinder() { }

        /// <summary>
        /// 提供默认的绑定器初始化一个 <see cref="DIModelBinder"/> 类的新实例。
        /// </summary>
        public DIModelBinder(IModelBinder defaultBinder)
        {
            if(defaultBinder == null) throw new ArgumentNullException(nameof(defaultBinder));
            this._defaultBinder = defaultBinder;
        }

        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>The bound value.</returns>
        public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if(this._defaultBinder == null || bindingContext.ModelType.IsInterface)
            {
                return Webx.Container.Get(bindingContext.ModelType);
            }
            return this._defaultBinder.BindModel(controllerContext, bindingContext);
        }
    }
}
