using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// 定义一个应用 URL 路径的提供程序。
    /// </summary>
    public interface IAppPathProvider
    {
        /// <summary>
        /// 获取指定页面的应用 Javascript URL 路径。 
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <returns>返回一个路径。</returns>
        string GetJsUrl(WebViewPage page);

        /// <summary>
        /// 获取指定文件名的完整 URL 路径。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <param name="parentPath">父级目录，可以为 null 值。</param>
        /// <param name="filename">文件名。</param>
        /// <returns>完整 URL 路径。</returns>
        string GetFullUrl(WebViewPage page, string parentPath, string filename);
    }

    /// <summary>
    /// 表示一个默认一个应用 URL 路径的提供程序。
    /// </summary>
    [SingletonMapping]
    public class AppPathProvider : IAppPathProvider
    {
        /// <summary>
        /// 获取指定文件名的完整 URL 路径。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <param name="parentPath">父级目录，可以为 null 值。</param>
        /// <param name="filename">文件名。</param>
        /// <returns>完整 URL 路径。</returns>
        public virtual string GetFullUrl(WebViewPage page, string parentPath, string filename)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));
            if(string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));
            if(filename[0] == '~' && filename[1] == '/') return page.Url.Content(filename);
            if(parentPath == null)
            {
                parentPath = "~/";
                switch(System.IO.Path.GetExtension(filename))
                {
                    case ".css":
                        parentPath = Convert.ToString(Webx.Container.Get("css-path") ?? "~/Content");
                        break;
                    case ".js":
                        parentPath = Convert.ToString(Webx.Container.Get("js-path") ?? "~/Scripts");
                        break;
                }
            }
            return page.Url.Content(System.IO.Path.Combine(parentPath, filename));
        }

        /// <summary>
        /// 获取指定页面的应用 Javascript URL 路径。 
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <returns>返回一个路径。</returns>
        public virtual string GetJsUrl(WebViewPage page)
        {
            var routeInfo = MvcClient.RouteInfo;
            var area = string.Empty;
            if(!routeInfo.Area.IsNull()) area = routeInfo.Area + "/";
            var fileName = routeInfo.Controller + "." + routeInfo.Action;
            return page.Url.Content($"~/Scripts/App/{area}{fileName}.js");
        }
    }
}
