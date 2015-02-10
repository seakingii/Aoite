using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示使用指定的路由来执行重定向的结果。
    /// </summary>
    public class RedirectToActionResult : RedirectToRouteResult
    {
        /// <summary>
        /// 初始化 <see cref="System.Web.Mvc.RedirectToActionResult"/>  类的新实例。
        /// </summary>
        /// <param name="action">操作。</param>
        /// <param name="values">一个对象，包含将作为元素添加到新集合中的属性。</param>
        public RedirectToActionResult(string action, object values = null)
            : this(action, null, values) { }

        /// <summary>
        /// 初始化 <see cref="System.Web.Mvc.RedirectToActionResult"/>  类的新实例。
        /// </summary>
        /// <param name="action">操作。</param>
        /// <param name="controller">控制器。</param>
        /// <param name="values">一个对象，包含将作为元素添加到新集合中的属性。</param>
        public RedirectToActionResult(string action, string controller, object values = null)
            : base(Create(action, controller, values)) { }

        private static RouteValueDictionary Create(string action, string controller, object values)
        {
            RouteValueDictionary dict = new RouteValueDictionary(values);
            if(controller != null) dict.Add("controller", controller);
            dict.Add("action", action);
            return dict;
        }
    }
}
