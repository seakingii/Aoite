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

        #region Controller

        private static readonly object Successfully = new { };

        /// <summary>
        /// 返回一个成功的结果。
        /// </summary>
        /// <param name="behavior">请求的行为。</param>
        /// <param name="controller">控制器。</param>
        /// <returns>将指定对象序列化为 JSON 格式的结果对象。</returns>
        public static JsonNetResult Success(this IController controller, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return Success(controller, Successfully, behavior);
        }
        /// <summary>
        /// 返回一个含值且成功的结果。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <param name="controller">控制器。</param>
        /// <param name="value">成功的值。</param>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>将指定对象序列化为 JSON 格式的结果对象。</returns>
        public static JsonNetResult Success<TValue>(this IController controller, TValue value, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return controller.Json2(value == null ? Successfully : new { value });
        }
        /// <summary>
        /// 返回一个错误的结果。
        /// </summary>
        /// <param name="controller">控制器。</param>
        /// <param name="message">错误的描述。</param>
        /// <param name="status">错误的状态。</param>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>将指定对象序列化为 JSON 格式的结果对象。</returns>
        public static JsonNetResult Faild(this IController controller, string message, int status = ResultStatus.Failed, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return controller.Json2(new { message, status });
        }

        /// <summary>
        /// 创建一个将指定对象序列化为 JavaScript 对象表示法 (JSON) 的 <see cref="JsonNetResult"/> 对象，请不要使用 <see cref="Web.Mvc.Controller.Json(object)"/>。
        /// </summary>
        /// <param name="controller">控制器。</param>
        /// <param name="data">要序列化的 JavaScript 对象图。</param>
        /// <param name="behavior">JSON 请求行为。</param>
        /// <returns>将指定对象序列化为 JSON 格式的结果对象。</returns>
        public static JsonNetResult Json2(this IController controller, object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonNetResult()
            {
                Data = data,
                JsonRequestBehavior = behavior,
            };
        }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="controller">控制器。</param>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        public static IDisposable AcquireLock<T>(this IController controller, TimeSpan? timeout = null)
        {
            return AcquireLock(controller, typeof(T).FullName, timeout);
        }
        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <param name="controller">控制器。</param>
        /// <param name="key">锁的键。</param>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        public static IDisposable AcquireLock(this IController controller, string key, TimeSpan? timeout = null)
        {
            return Webx.Container.Get<Aoite.CommandModel.ILockProvider>().Lock(key, timeout);
        }

        /// <summary>
        /// 获取指定数据类型键的原子递增序列。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="controller">控制器。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        public static long Increment<T>(this IController controller, long increment = 1)
        {
            return Increment(controller, typeof(T).FullName, increment);
        }
        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="controller">控制器。</param>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        public static long Increment(this IController controller, string key, long increment = 1)
        {
            return Webx.Container.Get<Aoite.CommandModel.ICounterProvider>().Increment(key, increment);
        }

        /// <summary>
        /// 开始事务模式。
        /// </summary>
        /// <returns>返回一个事务。</returns>
        public static ITransaction BeginTransaction(this IController controller)
        {
            return new Aoite.CommandModel.DefaultTransaction();
        }

        #endregion

        #region WebViewPage

        /// <summary>
        /// 渲染 Javascript 控制器。
        /// </summary>
        /// <param name="page">当前视图页。</param>
        /// <returns>返回一个 HTML 代码。</returns>
        public static IHtmlString RenderJsController(this WebViewPage page)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            return Optimization.Scripts.Render(AppendPathTick(Webx.Container.Get<IAppJsPathProvider>().GetUrl(page)));
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

        #endregion

        #region HtmlHelper

        /// <summary>
        /// 将指定的对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="helper">HTML 帮助器。</param>
        /// <param name="value">转换的对象。</param>
        /// <returns>返回一个 JSON 字符串。</returns>
        public static IHtmlString ToJson(this HtmlHelper helper, object value)
        {
            return new HtmlString(WebConfig.ToJson(value));
        }

        #endregion

        #region UrlHelper

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

        #endregion

        #region ModelStateDictionary

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

        #endregion
    }
}
