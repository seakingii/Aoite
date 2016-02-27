using Aoite.CommandModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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
            JsonConvert.DefaultSettings = () => DefaultJsonSetting;

            var container = Webx.Container;
            container.Add<IJsonProvider, JsonNetProvider>();
            container.Add<Web.Mvc.Async.IAsyncActionInvoker>(new Web.Mvc.AoiteAsyncActionInvoker());

            Web.Mvc.DependencyResolver.SetResolver(new Web.Mvc.ContainerDependencyResolver(Webx.Container));
            Web.Mvc.ModelBinders.Binders.DefaultBinder = new Web.Mvc.DIModelBinder(Web.Mvc.ModelBinders.Binders.DefaultBinder);
            Web.Mvc.ValueProviderFactories.Factories.Remove(Web.Mvc.ValueProviderFactories.Factories.OfType<Web.Mvc.JsonValueProviderFactory>().FirstOrDefault());
            Web.Mvc.ValueProviderFactories.Factories.Add(new Web.Mvc.JsonNetValueProviderFactory());
            Web.Mvc.GlobalFilters.Filters.Add(new Web.Mvc.Filters.MvcBasicActionFilterAttribute());
        }

        [SingletonMapping]
        class JsonNetProvider : IJsonProvider
        {
            public object Deserialize(string input, Type targetType)
            {
                return JsonConvert.DeserializeObject(input, targetType);
            }

            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        #region DefaultJsonSetting

        private static JsonSerializerSettings _DefaultJsonSetting = new JsonSerializerSettings
        {
            //TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            //DateFormatString = "yyyy'-'MM'-'dd HH':'mm':'ss",
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            //Converters = new List<JsonConverter>() { new JsonEnumConverter(), /*new ResultConverter()*/ },
            Converters = new List<JsonConverter>() { new Int64JsonConverter() }
        };

        class Int64JsonConverter : JsonConverter
        {

            public override bool CanConvert(Type objectType)
            {
                return objectType == Types.Int64 || objectType == typeof(Nullable<Int64>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jt = Newtonsoft.Json.Linq.JToken.ReadFrom(reader);
                if(jt.Type == Newtonsoft.Json.Linq.JTokenType.Null) return null;
                if(jt.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    var s = (string)jt;
                    return long.Parse(s);
                }
                return (long)jt;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if(value == null) writer.WriteNull();
                else
                {
                    var longValue = (long)value;
                    writer.WriteValue(value.ToString());
                    //if(longValue > 9007199254740992) writer.WriteValue(value.ToString());
                    //else writer.WriteValue(longValue);
                }
            }
        }
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
        public static string ToJson(object value)
        {
            return JsonConvert.SerializeObject(value/*, WebConfig.DefaultJsonSetting*/);
        }

        /// <summary>
        /// 表示 ASP.NET 单元测试模拟。
        /// </summary>
        public static class Mocks
        {
            /// <summary>
            /// 创建一个模拟的 MVC 控制器。
            /// </summary>
            /// <typeparam name="TController">控制的数据类型。</typeparam>     
            /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
            /// <returns>返回一个控制器的实例。</returns>
            public static TController Create<TController>(Action<MockExecutorFactory> mockFactoryCallback = null)
                      where TController : System.Web.Mvc.Controller, new()
            {
                return Create<TController>(null, mockFactoryCallback);
            }

            /// <summary>
            /// 创建一个模拟的 MVC 控制器。
            /// </summary>
            /// <typeparam name="TController">控制的数据类型。</typeparam>     
            /// <param name="user">当前已授权的登录用户。</param>
            /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
            /// <returns>返回一个控制器的实例。</returns>
            public static TController Create<TController>(object user, Action<MockExecutorFactory> mockFactoryCallback = null)
                where TController : Web.Mvc.Controller, new()
            {
                var httpRequest = new HttpRequest(string.Empty, "http://www.aoite.com/", string.Empty);
                var stringWriter = new IO.StringWriter();
                var httpResponce = new HttpResponse(stringWriter);
                var httpContext = new HttpContext(httpRequest, httpResponce);

                var sessionContainer = new SessionState.HttpSessionStateContainer("id", new SessionState.SessionStateItemCollection(),
                                                     new HttpStaticObjectsCollection(), 10, true,
                                                     HttpCookieMode.AutoDetect,
                                                     SessionState.SessionStateMode.InProc, false);

                httpContext.Items["AspSession"] = typeof(SessionState.HttpSessionState).GetConstructor(
                                                         BindingFlags.NonPublic | BindingFlags.Instance,
                                                         null, CallingConventions.Standard,
                                                         new[] { typeof(SessionState.HttpSessionStateContainer) },
                                                         null)
                                                    .Invoke(new object[] { sessionContainer });
                HttpContext.Current = httpContext;

                var appFactoryType = typeof(HttpContext).Assembly.GetType("System.Web.HttpApplicationFactory");

                object appFactory = DynamicFactory.CreateFieldGetter(appFactoryType.GetField("_theApplicationFactory", BindingFlags.NonPublic | BindingFlags.Static))(null);
                DynamicFactory.CreateFieldSetter(appFactoryType.GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance))(appFactory, HttpContext.Current.Application);

                var identityStore = new MockSessionIdentityStore(user);
                var container = ServiceFactory.CreateContainer(identityStore, mockFactoryCallback);
                container.Add<IIdentityStore>(identityStore);
                Webx.Container = container;
                TController c = new TController();
                c.ControllerContext = new Web.Mvc.ControllerContext(new Routing.RequestContext(new HttpContextWrapper(httpContext), new Routing.RouteData()), c);
                return c;
            }

            class MockSessionIdentityStore : IIdentityStore
            {
                public MockSessionIdentityStore(object user)
                {
                    this._User = user;
                }

                private object _User;
                public void Set(object user)
                {
                    this._User = user;
                }

                public object Get()
                {
                    return this._User;
                }

                public void Remove()
                {
                    this._User = null;
                }

                public object GetUser(IIocContainer container)
                {
                    return this.Get();
                }
            }
        }

        /// <summary>
        /// 表示 ASP.NET MVC 的相关配置。
        /// </summary>
        public static class Mvc
        {
            /// <summary>
            /// 在执行操作方法之前发生。
            /// </summary>
            public static event Web.Mvc.ActionExecutingEventHandler ActionExecuting;
            internal static Web.Mvc.ActionResult OnActionExecuting(object sender, Web.Mvc.ActionExecutingContext filterContext, bool allowAnonymous)
            {
                var handler = ActionExecuting;
                if(handler != null)
                {
                    var e = new Web.Mvc.ActionExecutingEventArgs(filterContext, allowAnonymous);
                    handler(sender, e);
                    return e.Result;
                }
                return null;
            }

            /// <summary>
            /// 在执行操作方法之前发生之后。
            /// </summary>
            public static event Web.Mvc.ActionExecutedEventHandler ActionExecuted;
            internal static Web.Mvc.ActionResult OnActionExecuted(object sender, Web.Mvc.ActionExecutedContext filterContext, bool allowAnonymous)
            {
                var handler = ActionExecuted;
                if(handler != null)
                {
                    var e = new Web.Mvc.ActionExecutedEventArgs(filterContext, allowAnonymous);
                    e.Result = filterContext.Result;
                    handler(sender, e);
                    return e.Result;
                }
                return null;
            }
        }
    }
}
