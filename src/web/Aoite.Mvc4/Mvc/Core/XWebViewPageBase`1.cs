using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示呈现使用 ASP.NET Razor 语法的视图所需的属性和方法。
    /// </summary>
    /// <typeparam name="TUser">用户的数据类型。</typeparam>
    public abstract class XWebViewPageBase<TUser> : WebViewPage
    {
        /// <summary>
        ///  获取用户。如果当前请求尚未授权，则为 null 值。
        /// </summary>
        public new TUser User { get { return MvcClient.Identity; } }
        /// <summary>
        /// 获取一个值，指示客户端是否已通过授权。
        /// </summary>
        public bool IsAuthorized { get { return MvcClient.IsAuthorized; } }
    }

    /// <summary>
    /// 表示呈现使用 ASP.NET Razor 语法的视图所需的属性和方法。
    /// </summary>
    /// <typeparam name="TUser">用户的数据类型。</typeparam>
    /// <typeparam name="TModel">视图数据模型的类型。</typeparam>
    public abstract class XWebViewPageBase<TUser, TModel> : WebViewPage<TModel>
    {
        /// <summary>
        ///  获取用户。如果当前请求尚未授权，则为 null 值。
        /// </summary>
        public new TUser User { get { return MvcClient.Identity; } }
        /// <summary>
        /// 获取一个值，指示客户端是否已通过授权。
        /// </summary>
        public bool IsAuthorized { get { return MvcClient.IsAuthorized; } }
    }
}
