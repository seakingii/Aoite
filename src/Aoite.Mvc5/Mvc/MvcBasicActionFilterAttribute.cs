﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Filters
{
    /// <summary>
    /// 表示基础筛选器特性。
    /// </summary>
    public class MvcBasicActionFilterAttribute : ActionFilterAttribute
    {
        private readonly static Type AllowAnonymousType = typeof(AllowAnonymousAttribute);

        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allowAnonymous = filterContext.ActionDescriptor.IsDefined(AllowAnonymousType, true)
                 || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(AllowAnonymousType, true);
            Webx.AllowAnonymous = allowAnonymous;

            var result = WebConfig.Mvc.OnActionExecuting(filterContext.Controller, filterContext, allowAnonymous);
            if(result != null) filterContext.Result = result;


            if(filterContext.Result is HttpUnauthorizedResult && !Webx.IsAjaxRequest)
            {
                var loginUrl = Webx.Container.Get("login.url", filterContext.RequestContext.HttpContext.Request.RawUrl) as string;
                if(loginUrl != null) filterContext.Result = new RedirectResult(loginUrl);
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = WebConfig.Mvc.OnActionExecuted(filterContext.Controller, filterContext, MvcClient.AllowAnonymous);
            if(result != null) filterContext.Result = result;

            var jsonResult = filterContext.Result as JsonResult;

            if(jsonResult != null && !(jsonResult is JsonNetResult))
            {
                filterContext.Result = new JsonNetResult
                {
                    ContentEncoding = jsonResult.ContentEncoding,
                    ContentType = jsonResult.ContentType,
                    Data = jsonResult.Data,
                    JsonRequestBehavior = jsonResult.JsonRequestBehavior
                };
            }


            base.OnActionExecuted(filterContext);
        }

    }
}
