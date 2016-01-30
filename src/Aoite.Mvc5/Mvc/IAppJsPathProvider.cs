using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 定义一个应用 Javascript 路径的提供程序。
    /// </summary>
    public interface IAppJsPathProvider
    {
        /// <summary>
        /// 获取指定页面的应用 Javascript 路径。 
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <returns>返回一个路径。</returns>
        string GetUrl(WebViewPage page);
    }

    [SingletonMapping]
    class AppJsPathProvider : IAppJsPathProvider
    {
        public string GetUrl(WebViewPage page)
        {
            var routeInfo = MvcClient.RouteInfo;
            var area = string.Empty;
            if(!routeInfo.Area.IsNull()) area = routeInfo.Area + "/";
            var fileName = routeInfo.Controller + "." + routeInfo.Action;
            return page.Url.Content($"~/Scripts/App/{area}{fileName}.js");
        }
    }
}
