using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Mvc 框架的扩展。
    /// </summary>
    public static class AoiteMvcExtensions
    {
        static readonly long JsTimeTicks = DateTime.Now.Ticks;
        /// <summary>
        /// 渲染 Javascript 控制器。
        /// </summary>
        /// <returns>返回一个 HTML 代码。</returns>
        public static MvcHtmlString RenderJsController(this WebViewPage page)
        {
            BuildManagerCompiledView view;
            if(page != null && (view = page.ViewContext.View as BuildManagerCompiledView) != null)
            {
                var viewPath = (page.ViewContext.View as RazorView).ViewPath;
                return MvcHtmlString.Create("<script src=\""
                    + page.Url.Content(System.IO.Path.ChangeExtension(viewPath, "js")) + "?t=" + JsTimeTicks + "\"></script>");
            }
            return null;
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
        {
            return url.Action(actionName, new { id = id });
        }

        /// <summary>
        /// 使用指定的操作名称、控制器名称和 Id 值生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="actionName">操作方法的名称。</param>
        /// <param name="controllerName">控制器的名称。</param>
        /// <param name="id">操作方法的 ID 值。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string ActionById(this UrlHelper url, string actionName, string controllerName, object id)
        {
            return url.Action(actionName, controllerName, new { id = id });
        }

        /// <summary>
        /// 使用指定的控制器名称生成操作方法的完全限定 URL。
        /// </summary>
        /// <param name="url">应用程序内的 ASP.NET MVC 生成 URL 的方法。</param>
        /// <param name="controllerName">控制器的名称。</param>
        /// <returns>操作方法的完全限定 URL。</returns>
        public static string Controller(this UrlHelper url, string controllerName)
        {
            return url.Action("Index", controllerName);
        }

        private class CustomResult
        {
            public string Message { get; set; }
            public int Status { get; set; }
            public object Value { get; set; }
        }

        /// <summary>
        /// 将当前结果转换为 <see cref="System.Web.Mvc.JsonResult"/> 类的新实例。
        /// </summary>
        /// <param name="result">结果。</param>
        /// <returns>返回一个 <see cref="System.Web.Mvc.JsonResult"/> 类的新实例。</returns>
        public static JsonResult ToJson(this Result result)
        {
            var cr = new CustomResult { Status = result.Status };
            if(result.IsFailed) cr.Message = result.Message;

            return new JsonResult() { Data = cr };
        }

        /// <summary>
        /// 将当前结果转换为 <see cref="System.Web.Mvc.JsonResult"/> 类的新实例。
        /// </summary>
        /// <typeparam name="TValue">结果值的数据类型。</typeparam>
        /// <param name="result">结果。</param>
        /// <returns>返回一个 <see cref="System.Web.Mvc.JsonResult"/> 类的新实例。</returns>
        public static JsonResult ToJson<TValue>(this Result<TValue> result)
        {
            var cr = new CustomResult { Status = result.Status, Value = result.Value };
            if(result.IsFailed) cr.Message = result.Message;

            return new JsonResult() { Data = cr };
        }
    }
}
