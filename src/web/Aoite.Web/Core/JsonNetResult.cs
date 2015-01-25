using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示一个类，该类用于将 JSON 格式的内容发送到响应（请不要使用 <see cref="System.Web.Mvc.JsonResult"/> 类）。
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        /// <summary>
        /// 初始化 <see cref="System.Web.Mvc.JsonNetResult"/> 类的新实例。
        /// </summary>
        public JsonNetResult() { }

        /// <summary>
        /// 初始化 <see cref="System.Web.Mvc.JsonNetResult"/> 类的新实例。
        /// </summary>
        /// <param name="data">数据。</param>
        public JsonNetResult(object data)
        {
            base.Data = data;
        }

        /// <summary>
        /// 通过从 <see cref="System.Web.Mvc.ActionResult"/> 类继承的自定义类型，启用对操作方法结果的处理。
        /// </summary>
        /// <param name="context">执行结果时所处的上下文。</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if(this.Data == null) return;
            if(context == null)
                throw new ArgumentNullException("context");
            if(this.JsonRequestBehavior == JsonRequestBehavior.DenyGet
                && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON 不允许以 GET 方法调用！");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if(this.ContentEncoding != null) response.ContentEncoding = this.ContentEncoding;

            response.Write(this.Data.ToJson());
        }
    }
}
