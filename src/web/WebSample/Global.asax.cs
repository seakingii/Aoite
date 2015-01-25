using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebSample
{
    public class MvcApplication : System.Web.XHttpApplication
    {
        protected virtual void Application_Start()
        {
            WebConfig.Mvc.ActionExecuting += (sender, e) =>
            {
                if(e.AllowAnonymous || MvcClient.IsAuthorized) return;

                #region 会话过期

                var logoutUrl = "~/Member/Login";
                if(MvcClient.IsJsonResponse)
                {
                    HttpContext.Current.Response.StatusCode = 401;
                    e.Result = new JsonResult()
                    {
                        Data = new
                        {
                            message = "您的登录已超时...",
                            state = 401,
                            value = logoutUrl
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    e.Result = new RedirectResult(logoutUrl);
                }

                #endregion
            };
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}