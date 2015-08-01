using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 执行操作方法的事件参数。
    /// </summary>
    public abstract class ActionExecuteEventArgs : EventArgs
    {
        /// <summary>
        /// 设置或获取由操作方法返回的结果。
        /// </summary>
        public ActionResult Result { get; set; }

        private bool _AllowAnonymous;
        /// <summary>
        /// 获取一个值，指示当前控制器或操作是否允许匿名访问。
        /// </summary>
        public bool AllowAnonymous { get { return this._AllowAnonymous; } }

        internal ActionExecuteEventArgs(bool allowAnonymous)
        {
            this._AllowAnonymous = allowAnonymous;
        }
    }

    /// <summary>
    /// 执行操作方法之前发生的事件参数。
    /// </summary>
    public class ActionExecutingEventArgs : ActionExecuteEventArgs
    {
        private ActionExecutingContext _FilterContext;
        /// <summary>
        /// 获取筛选器上下文。
        /// </summary>
        public ActionExecutingContext FilterContext
        {
            get
            {
                return this._FilterContext;
            }
        }

        internal ActionExecutingEventArgs(ActionExecutingContext filterContext, bool allowAnonymous)
            : base(allowAnonymous)
        {
            this._FilterContext = filterContext;
        }
    }

    /// <summary>
    /// 执行操作方法之前发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ActionExecutingEventHandler(object sender, ActionExecutingEventArgs e);

    /// <summary>
    /// 执行操作方法之后发生的事件参数。
    /// </summary>
    public class ActionExecutedEventArgs : ActionExecuteEventArgs
    {
        private ActionExecutedContext _FilterContext;
        /// <summary>
        /// 获取筛选器上下文。
        /// </summary>
        public ActionExecutedContext FilterContext
        {
            get
            {
                return this._FilterContext;
            }
        }

        internal ActionExecutedEventArgs(ActionExecutedContext filterContext, bool allowAnonymous)
            : base(allowAnonymous)
        {
            this._FilterContext = filterContext;
        }
    }

    /// <summary>
    /// 执行操作方法之后发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ActionExecutedEventHandler(object sender, ActionExecutedEventArgs e);
}
