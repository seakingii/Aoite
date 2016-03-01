using Aoite.CommandModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web
{
    ///// <summary>
    ///// 定义 ASP.NET 应用程序中的所有应用程序对象共有的方法、属性和事件。此类是用户在 Global.asax 文件中所定义的应用程序的基类。
    ///// </summary>
    //public class XHttpApplication : System.Web.HttpApplication
    //{
    //    /// <summary>
    //    /// 初始化一个 <see cref="System.Web.XHttpApplication"/> 类的新实例。
    //    /// </summary>
    //    public XHttpApplication() { }

    //    /// <summary>
    //    /// 在添加所有事件处理程序模块之后执行自定义初始化代码。
    //    /// </summary>
    //    public override void Init()
    //    {
    //        this.Error += (ss, ee) =>
    //        {
    //            var exception = this.Server.GetLastError();
    //            var httpException = exception as HttpException;
    //            if(httpException != null && httpException.GetHttpCode() == 404) return;

    //            GA.OnGlobalError(this.Context, exception);
    //        };
    //        this.EndRequest += (ss, ee) =>
    //        {
    //            GA.ResetContexts();
    //        };

    //        base.Init();
    //    }
    //}
}
