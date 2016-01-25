using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// MVC 客户端上下文。
    /// </summary>
    public class MvcClient : Webx
    {
        private const string UrlHelperName = "$MvcClient:URL_HELPER";
        /// <summary>
        /// 获取用于使用路由来生成 URL 的 URL 帮助器对象。
        /// </summary>
        public static UrlHelper Url =>  GetTemp(UrlHelperName, () => new UrlHelper(HttpContext.Current.Request.RequestContext));

        private const string RouteInfoName = "$MvcClient:ROUTE_INFO";
        /// <summary>
        /// 获取当前请求的路由信息。
        /// </summary>
        public static RequestRouteInfo RouteInfo => GetTemp(RouteInfoName, () => new RequestRouteInfo(HttpContext.Current.Request.RequestContext.RouteData));
    }
}
