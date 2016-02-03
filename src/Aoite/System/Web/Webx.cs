using Aoite.CommandModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Web
{
    /// <summary>
    /// 表示一个 Web Application 的增强功能。
    /// </summary>
    public class Webx
    {
        #region Temp Values

        /// <summary>
        /// 指定一个名称，获取当前请求的临时数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <returns>如果存在返回值，否则返回默认值。</returns>
        public static T GetTemp<T>(string name) => GetTemp<T>(name, null);
        /// <summary>
        /// 指定一个名称和默认值，获取当前请求的临时数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="defaultValue">自定义的默认值。</param>
        /// <returns>如果存在返回值，否则返回默认值。</returns>
        public static T GetTemp<T>(string name, T defaultValue) => GetTemp<T>(name, null);

        /// <summary>
        /// 指定一个名称和默认值回调方法，获取当前请求的临时数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="defaultValueCallback">自定义的默认值的回调方法。</param>
        /// <returns>如果存在返回值，否则执行回调方法并返回默认值。</returns>
        public static T GetTemp<T>(string name, Func<T> defaultValueCallback)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var items = Items;
            var value = items[name];
            if(value == null)
            {
                if(defaultValueCallback == null) return default(T);
                var tValue = defaultValueCallback();
                items[name] = tValue;
                return tValue;
            }

            return (T)value;
        }

        /// <summary>
        /// 设置当前请求的临时数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        /// <returns>设置的值。</returns>
        public static T SetTemp<T>(string name, T value)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var items = Items;
            if(value == null) items.Remove(name);
            else items[name] = value;
            return value;
        }

        #endregion

        #region Url & Path

        /// <summary>
        /// 返回一个包含内容 URL 的字符串。
        /// </summary>
        /// <param name="contentPath">内容路径。</param>
        /// <returns>包含内容 URL 的字符串。</returns>
        public static string MapUrl(string contentPath)
        {
            if(string.IsNullOrWhiteSpace(contentPath)) throw new ArgumentNullException(nameof(contentPath));
            var path = HttpRuntime.AppDomainAppVirtualPath ?? "/";
            return VirtualPathUtility.Combine(path, VirtualPathUtility.ToAbsolute(contentPath, path));
        }

        /// <summary>
        /// 将虚拟路径映射到服务器上的物理路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径（绝对路径或相对路径）。</param>
        /// <returns>由 <paramref name="virtualPath"/> 指定的服务器物理路径。</returns>
        public static string MapPath(string virtualPath) => Hosting.HostingEnvironment.MapPath(virtualPath);

        #endregion

        #region Scripts

        private const string SCRIPT_STRING_BUILDER = "$Webx:SCRIPT_STRING_BUILDER$";
        private readonly static IHtmlString EmptyHtmlString = new HtmlString(string.Empty);
        /// <summary>
        /// 添加客户端脚本。
        /// </summary>
        /// <param name="scripts">脚本的内容。</param>
        public static void AppendScripts(string scripts)
        {
            if(string.IsNullOrWhiteSpace(scripts)) return;
            var scriptBuilder = GetTemp<Text.StringBuilder>(SCRIPT_STRING_BUILDER, () => new Text.StringBuilder());

            using(var reader = new System.IO.StringReader(scripts))
            {
                string line = reader.ReadLine();
                while(line != null)
                {
                    scriptBuilder.Append("            ");
                    scriptBuilder.AppendLine(line);
                    line = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// 呈现所有已添加的脚本，并清空脚本。
        /// </summary>
        /// <returns>脚本字符串。</returns>
        public static IHtmlString ReaderScripts()
        {
            var scriptBuilder = GetTemp<Text.StringBuilder>(SCRIPT_STRING_BUILDER);
            if(scriptBuilder == null) return EmptyHtmlString;
            var content = scriptBuilder.ToString();
            return new HtmlString(
@"    <script type=""text/javascript"">
        $(function () {
" + content + @"
        });
    </script>");
        }

        #endregion

        #region Commom

        private const string IsAjaxRequestName = "$Webx:IS_AJAX_REQUEST";
        /// <summary>
        /// 设置或获取一个值，确定指定的 HTTP 请求是否为 AJAX 请求。
        /// </summary>
        public static bool IsAjaxRequest
        {
            get
            {
                var isJsonResult = GetTemp<bool?>(IsAjaxRequestName, defaultValue: null);
                if(isJsonResult == null)
                {
                    var request = HttpContext.Current.Request;
                    isJsonResult = ((request["X-Requested-With"] == "XMLHttpRequest")
                        || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest")));

                    SetTemp(IsAjaxRequestName, isJsonResult);
                }
                return isJsonResult.Value;
            }
            set { SetTemp(IsAjaxRequestName, value); }
        }

        #endregion

        #region Identity

        /// <summary>
        /// 获取一个基于 <see cref="HttpContext.Items"/> 的存取器。
        /// </summary>
        public static IItemsAccessor Items => Container.Get<IItemsAccessor>();
        /// <summary>
        /// 获取一个基于 <see cref="HttpContext.Session"/> 的存取器。
        /// </summary>
        public static ISessionAccessor Session => Container.Get<ISessionAccessor>();
        /// <summary>
        /// 获取一个基于 Cookie 的存取器。
        /// </summary>
        public static ICookieAccessor Cookie => Container.Get<ICookieAccessor>();


        private static IIocContainer SetContainer(IIocContainer value)
        {
            var config = value.Get("$AppSettings") as NameValueCollection
                  ?? System.Web.Configuration.WebConfigurationManager.AppSettings
                  ?? new NameValueCollection();

            var redisAddress = config.Get("redis.address");
            if(!string.IsNullOrWhiteSpace(redisAddress))
            {
                var sp = redisAddress.Split(':');
                if(sp.Length != 2) throw new ArgumentException("非法的 Redis 的连接地址 {0}。".Fmt(redisAddress));
                Aoite.Redis.RedisManager.DefaultAddress = new Aoite.Net.SocketInfo(sp[0], int.Parse(sp[1]));
            }
            Aoite.Redis.RedisManager.DefaultPassword = config.Get("redis.password");

            if(config.Get<bool>("redis.enabled")) value.Add<IRedisProvider>(new RedisProvider(value));

            if(!value.Contains<IUserFactory>(true)) value.Add<IUserFactory>(value.Get<IIdentityStore>());

            _WebContainer = value;
            return value;
        }


        private static IIocContainer _WebContainer;

        /// <summary>
        /// 获取或设置用于 Webx 的服务容器。
        /// </summary>
        public static IIocContainer Container
        {
            get
            {
                var container = _WebContainer;
                if(container == null) return _WebContainer ?? SetContainer(ObjectFactory.Global);
                return container;
            }
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(value));
                SetContainer(value);
            }
        }

        /// <summary>
        /// 获取一个值，指示当前请求是否已通过授权。
        /// </summary>
        public static bool IsAuthorized { get { return Identity != null; } }

        private const string AllowAnonymousName = "$Webx:ALLOW_ANONYMOUS";
        /// <summary>
        /// 获取或设置一个值，指示当前请求是否允许匿名访问。
        /// </summary>
        public static bool AllowAnonymous { get { return GetTemp(AllowAnonymousName, false); } set { SetTemp(AllowAnonymousName, value); } }

        private const string IdentityName = "$Webx:IDENTITY";
        /// <summary>
        ///  获取或设置客户端唯一标识，如果上下文缓存不存在，则尝试从当前请求中获取。
        /// </summary>
        /// <returns>客户端唯一标识。</returns>
        public static dynamic Identity
        {
            get { return GetTemp<object>(IdentityName, Container.Get<IIdentityStore>().Get); }
            set
            {
                var store = Container.Get<IIdentityStore>();
                object v = value;
                if(v == null)
                {
                    store.Remove();
                    SetTemp<object>(IdentityName, null);
                }
                else
                {
                    store.Set(v);
                    SetTemp(IdentityName, v);
                }
            }
        }

        [SingletonMapping]
        internal class SessionIdentityStore : IIdentityStore
        {
            ISessionAccessor _session;
            public SessionIdentityStore(ISessionAccessor session)
            {
                if(session == null) throw new ArgumentNullException(nameof(session));
                this._session = session;
            }
            public void Set(object user)
            {
                this._session[IdentityName] = user;
            }

            public object Get()
            {
                return this._session[IdentityName];
            }

            public void Remove()
            {
                this._session.Remove(IdentityName);
            }

            public object GetUser(IIocContainer container)
            {
                return this.Get();
            }
        }

        #endregion
    }

}
