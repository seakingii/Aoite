using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web
{
    ///// <summary>
    ///// 表示一个异常处理的 HTTP 模块。
    ///// </summary>
    //public class ErrorHandlerHttpModule : IHttpModule
    //{
    //    private readonly Func<int, string> GetHttpCodeUrl;
    //    /// <summary>
    //    /// 初始化一个 <see cref="ErrorHandlerHttpModule"/> 类的新实例。
    //    /// </summary>
    //    /// <param name="getHttpCodeUrl">获取指定 HTTP 状态码的 URL 地址。</param>
    //    public ErrorHandlerHttpModule(Func<int, string> getHttpCodeUrl)
    //    {
    //        if(getHttpCodeUrl == null) throw new ArgumentNullException(nameof(getHttpCodeUrl));
    //        this.GetHttpCodeUrl = getHttpCodeUrl;
    //    }

    //    /// <summary>
    //    /// 初始化模块，并使其为处理请求做好准备。
    //    /// </summary>
    //    /// <param name="context">一个 <see cref="HttpApplication"/>，它提供对 ASP.NET 应用程序内所有应用程序对象的公用的方法、属性和事件的访问。</param>
    //    public virtual void Init(HttpApplication context)
    //    {
    //        context.EndRequest += Application_EndRequest;
    //    }

    //    /// <summary>
    //    /// 处置由实现 <see cref="IHttpModule"/> 的模块使用的资源（内存除外）。
    //    /// </summary>
    //    public virtual void Dispose() { }

    //    private string GetExecutionFilePath(HttpContext context)
    //    {
    //        var path = context.Request.AppRelativeCurrentExecutionFilePath;
    //        if(path[0] == '~') path = path.Substring(1);
    //        return path;
    //    }
    //    /// <summary>
    //    /// 在 ASP.NET 响应请求时作为 HTTP 执行管线链中的最后一个事件发生。
    //    /// </summary>
    //    /// <param name="sender">事件源。</param>
    //    /// <param name="e">不包含事件数据的对象。</param>
    //    protected virtual void Application_EndRequest(object sender, EventArgs e)
    //    {
    //        var context = HttpContext.Current;
    //        var exception = context.Error;
    //        var statusCode = exception != null ? 500 : context.Response.StatusCode;

    //        var httpException = exception as HttpException;
    //        if(httpException != null)
    //        {
    //            statusCode = httpException.GetHttpCode();
    //        }

    //        if(statusCode != 200)
    //        {
    //            var url = GetHttpCodeUrl(statusCode);
    //            if(!string.Equals(GetExecutionFilePath(context), url, StringComparison.CurrentCultureIgnoreCase))
    //            {
    //                context.Server.ClearError();
    //                context.Response.TrySkipIisCustomErrors = true;
    //            }
    //            else if(url != null)
    //            {
    //                context.Response.Clear();
    //                context.Response.TrySkipIisCustomErrors = true;

    //                context.Server.ClearError();

    //                if(HttpRuntime.UsingIntegratedPipeline)
    //                {
    //                    context.Server.TransferRequest(url, true);
    //                }
    //                else
    //                {
    //                    context.RewritePath(url, false);
    //                    IHttpHandler httpHandler = new Mvc.MvcHttpHandler();
    //                    httpHandler.ProcessRequest(context);
    //                }
    //                context.Response.StatusCode = statusCode;
    //            }
    //        }

    //    }
    //}
}
