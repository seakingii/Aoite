using Aoite.CommandModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Web.PreApplicationStartMethod(typeof(System.Web.WebConfig), "Startup")]
namespace System.Web
{
    /// <summary>
    /// 表示全局配置文件。
    /// </summary>
    public static class WebConfig
    {
        /// <summary>
        /// 启动 ASP.NET。
        /// </summary>
        public static void Startup()
        {
            System.Web.Mvc.GlobalFilters.Filters.Add(new System.Web.Mvc.Filters.MvcBasicActionFilterAttribute());
        }

        #region DefaultJsonSetting

        private static JsonSerializerSettings _DefaultJsonSetting = new JsonSerializerSettings
        {
            //TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            //DateFormatString = "yyyy'-'MM'-'dd HH':'mm':'ss",
            ConstructorHandling = Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new List<JsonConverter>() { new JsonEnumConverter() }
        };

        #endregion

        /// <summary>
        /// 设置或获取默认 JSON 序列化的配置。
        /// <para>默认情况下，忽略空值和默认值，并且支持枚举、属性名首字母自动小写（如 ID=id，IDString=idString，UserName=userName）。</para>
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSetting
        {
            get
            {
                return _DefaultJsonSetting;
            }
            set
            {
                if(value == null) return;
                _DefaultJsonSetting = value;
            }
        }

        /// <summary>
        /// 将指定的对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="value">转换的对象。</param>
        /// <returns>返回一个 JSON 字符串。</returns>
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, WebConfig.DefaultJsonSetting);
        }
        /// <summary>
        /// 表示 ASP.NET MVC 的相关配置。
        /// </summary>
        public static class Mvc
        {
            /// <summary>
            /// 在执行操作方法之前发生。
            /// </summary>
            public static event System.Web.Mvc.ActionExecutingEventHandler ActionExecuting;
            internal static System.Web.Mvc.ActionResult OnActionExecuting(object sender, System.Web.Mvc.ActionExecutingContext filterContext, bool allowAnonymous)
            {
                var handler = ActionExecuting;
                if(handler != null)
                {
                    var e = new System.Web.Mvc.ActionExecutingEventArgs(filterContext, allowAnonymous);
                    handler(sender, e);
                    return e.Result;
                }
                return null;
            }

            /// <summary>
            /// 在执行操作方法之前发生之后。
            /// </summary>
            public static event System.Web.Mvc.ActionExecutedEventHandler ActionExecuted;
            internal static System.Web.Mvc.ActionResult OnActionExecuted(object sender, System.Web.Mvc.ActionExecutedContext filterContext, bool allowAnonymous)
            {
                var handler = ActionExecuted;
                if(handler != null)
                {
                    var e = new System.Web.Mvc.ActionExecutedEventArgs(filterContext, allowAnonymous);
                    handler(sender, e);
                    return e.Result;
                }
                return null;
            }
        }
    }
}
