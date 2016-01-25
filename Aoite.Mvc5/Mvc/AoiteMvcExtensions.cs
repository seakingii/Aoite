using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public interface IAppJsPathProvider
    {
        string GetUrl(WebViewPage page);
    }
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

    /// <summary>
    /// Mvc 框架的扩展。
    /// </summary>
    public static class AoiteMvcExtensions
    {
        static readonly long JsTimeTicks = DateTime.Now.Ticks;


        /// <summary>
        /// 渲染 Javascript 控制器。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <returns>返回一个 HTML 代码。</returns>
        public static IHtmlString RenderJsController(this WebViewPage page)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            return Optimization.Scripts.Render(AppendPathTick(Webx.Container.Get<IAppJsPathProvider>().GetUrl(page)));

            //BuildManagerCompiledView view;
            //if(page != null && (view = page.ViewContext.View as BuildManagerCompiledView) != null)
            //{
            //    //page.ViewContext.View as RazorView
            //    var viewPath = (page.ViewContext.View as RazorView).ViewPath;
            //    return MvcHtmlString.Create("<script src=\""
            //        + page.Url.Content(IO.Path.ChangeExtension(viewPath, "js")) + "?t=" + JsTimeTicks + "\"></script>");
            //}
            //return null;
        }

        private static string[] AppendPathTick(params string[] paths)
        {
            for(int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                if(path.Contains("?")) path += "&";
                else path += "?";

                paths[i] = path + "t=" + JsTimeTicks;
            }

            return paths;
        }

        /// <summary>
        /// 输出脚本文件。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <param name="paths">脚本路径的数组。</param>
        /// <returns>返回一批 HTML SCRIPT 代码。</returns>
        public static IHtmlString RenderScripts(this WebViewPage page, params string[] paths)
            => Optimization.Scripts.Render(AppendPathTick(paths));

        /// <summary>
        /// 输出样式文件。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <param name="paths">脚本路径的数组。</param>
        /// <returns>返回一个 HTML LINK 代码。</returns>
        public static IHtmlString RenderStyles(this WebViewPage page, params string[] paths)
            => Optimization.Styles.Render(AppendPathTick(paths));

        /// <summary>
        /// 将指定的对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="helper">HTML 帮助器。</param>
        /// <param name="value">转换的对象。</param>
        /// <returns>返回一个 JSON 字符串。</returns>
        public static IHtmlString ToJson(this HtmlHelper helper, object value)
        {
            return new HtmlString(value.ToJson());
        }

        /// <summary>
        /// 使用指定分部页视图名称生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="partailViewName">分部页视图的名称。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string Partail(this UrlHelper url, string partailViewName)
        {
            return ActionById(url, "Partail", partailViewName);
        }

        /// <summary>
        /// 使用指定的操作名称和 Id 值生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="actionName">操作方法的名称。</param>
        /// <param name="id">操作方法的 ID 值。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string ActionById(this UrlHelper url, string actionName, object id)
            => url.Action(actionName, new { id = id });

        /// <summary>
        /// 使用指定的操作名称、控制器名称和 Id 值生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="actionName">操作方法的名称。</param>
        /// <param name="controllerName">控制器的名称。</param>
        /// <param name="id">操作方法的 ID 值。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string ActionById(this UrlHelper url, string actionName, string controllerName, object id)
            => url.Action(actionName, controllerName, new { id = id });

        /// <summary>
        /// 使用指定的控制器名称生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="controllerName">控制器的名称。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string Controller(this UrlHelper url, string controllerName)
            => url.Action("Index", controllerName);

        /// <summary>
        /// 获取第一个错误。
        /// </summary>
        /// <param name="modelState">一个 <see cref="System.Web.Mvc.ModelStateDictionary"/>。</param>
        /// <returns>返回一个 null 值，或首个错误的内容。</returns>
        public static string FirstError(this ModelStateDictionary modelState)
            => modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();

        /// <summary>
        /// 获取 <see cref="ModelStateDictionary"/> 所有错误的分隔符。
        /// </summary>
        public static string AllErrorsJoinSeparator = "\n";

        /// <summary>
        /// 获取所有错误，默认以“\n”合并。
        /// </summary>
        /// <param name="modelState">一个 <see cref="System.Web.Mvc.ModelStateDictionary"/>。</param>
        /// <param name="separator">分隔符。</param>
        /// <returns>返回错误的合并内容。</returns>
        public static string AllErrors(this ModelStateDictionary modelState, string separator = null)
            => string.Join(separator ?? AllErrorsJoinSeparator, modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
    }
}
