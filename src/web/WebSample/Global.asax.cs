using System;
using System.Collections.Generic;
using System.IO;
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
        private void InitializeDemo()
        {
            var localDbPath = MvcClient.MapPath(Db.Engine.ConnectionString);
            Db.Engine.ConnectionString = "Data Source='" + localDbPath + "'";
            Db.Readonly.ConnectionString = Db.Engine.ConnectionString;
            if(!File.Exists(localDbPath))
            {
                (Db.Engine as Aoite.Data.MsSqlCeEngine).CreateDatabase();
                var scriptManger = new Aoite.Data.SqlScriptsManager();
                scriptManger.ParseFolder(Path.GetDirectoryName(localDbPath));
                foreach(var item in scriptManger)
                {
                    Db.Execute(Aoite.Data.MsCeTestManager.ReplaceVarchar(item.Value)).ToNonQuery().ThrowIfFailded();
                }
            }
        }

        protected virtual void Application_Start()
        {
            InitializeDemo();
            WebConfig.Mvc.ActionExecuting += (sender, e) =>
            {
                if(e.AllowAnonymous || MvcClient.IsAuthorized) return;

                #region 会话过期

                var logoutUrl = MvcClient.MapUrl("~/Account/Login");
                if(MvcClient.IsJsonResponse)
                {
                    HttpContext.Current.Response.StatusCode = 401;
                    e.Result = new JsonResult()
                    {
                        Data = new
                        {
                            message = "您的登录已超时...",
                            status = 401,
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