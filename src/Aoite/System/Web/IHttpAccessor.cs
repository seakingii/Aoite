using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web
{
    /// <summary>
    /// 定义一个 HTPP 的存取器。
    /// </summary>
    public interface IHttpAccessor
    {
        /// <summary>
        /// 设置或获取指定名称的值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>值。</returns>
        object this[string name] { get; set; }
        /// <summary>
        /// 设置指定名称的值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        void Set(string name, object value);
        /// <summary>
        /// 获取指定名称的值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>值。</returns>
        object Get(string name);
        /// <summary>
        /// 移除指定名称的值。
        /// </summary>
        /// <param name="name">名称。</param>
        void Remove(string name);
        /// <summary>
        /// 清空存取器的所有项。
        /// </summary>
        void Clear();
    }

    abstract class HttpAccessorBase : IHttpAccessor
    {
        public object this[string name]
        {
            get { return this.Get(name); }
            set
            {
                if(value == null) this.Remove(name);
                else this.Set(name, value);
            }
        }

        public abstract void Clear();

        public abstract object Get(string name);
        public abstract void Remove(string name);

        public abstract void Set(string name, object value);
    }

    /// <summary>
    /// 表示一个基于 <see cref="HttpContext.Items"/> 的存取器。
    /// </summary>
    [DefaultMapping(typeof(HttpItemsAccessor))]
    public interface IItemsAccessor : IHttpAccessor { }

    [SingletonMapping]
    class HttpItemsAccessor : HttpAccessorBase, IItemsAccessor
    {
        public override void Clear()
        {
            HttpContext.Current.Items.Clear();
        }

        public override object Get(string name)
        {
            return HttpContext.Current.Items[name];
        }

        public override void Set(string name, object value)
        {
            HttpContext.Current.Items[name] = value;
        }
        public override void Remove(string name)
        {
            HttpContext.Current.Items.Remove(name);
        }
    }

    /// <summary>
    /// 表示一个基于 <see cref="HttpContext.Session"/> 的存取器。
    /// </summary>
    [DefaultMapping(typeof(HttpSessionAccessor))]
    public interface ISessionAccessor : IHttpAccessor { }
    [SingletonMapping]
    class HttpSessionAccessor : HttpAccessorBase, ISessionAccessor
    {
        public override void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public override object Get(string name)
        {
            return HttpContext.Current.Session[name];
        }

        public override void Set(string name, object value)
        {
            HttpContext.Current.Session[name] = value;
        }
        public override void Remove(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }
    }

    /// <summary>
    /// 表示一个基于 Cookie 的存取器。
    /// </summary>
    [DefaultMapping(typeof(HttpCookieAccessor))]
    public interface ICookieAccessor : IHttpAccessor
    {
        /// <summary>
        /// 新建或更新客户端的 Cookie。
        /// </summary>
        /// <param name="name">新 Cookie 的名称。</param>
        /// <param name="value">新 Cookie 的值。如果值为 null 值，则表示移除该项。</param>
        /// <param name="path">要与当前 Cookie 一起传输的虚拟路径。</param>
        /// <param name="httpOnly">指定 Cookie 是否可通过客户端脚本访问。</param>
        void Set(string name, string value, string path = null, bool httpOnly = false);
        /// <summary>
        /// 新建或更新客户端的 Cookie。
        /// </summary>
        /// <param name="name">新 Cookie 的名称。</param>
        /// <param name="value">新 Cookie 的值。如果值为 null 值，则表示移除该项。</param>
        /// <param name="expires">此 Cookie 的过期日期和时间。</param>
        /// <param name="path">要与当前 Cookie 一起传输的虚拟路径。</param>
        /// <param name="httpOnly">指定 Cookie 是否可通过客户端脚本访问。</param>
        void Set(string name, string value, DateTime expires, string path = null, bool httpOnly = false);
    }
    [SingletonMapping]
    class HttpCookieAccessor : HttpAccessorBase, ICookieAccessor
    {
        public override void Clear()
        {
            var ctx = HttpContext.Current;
            var response = ctx.Response;
            foreach(var key in ctx.Request.Cookies.AllKeys)
            {
                response.Cookies[key].Expires = DateTime.Now.AddYears(-1);
            }
        }

        public override object Get(string name)
        {
            var cookie = HttpContext.Current.Request.Cookies[name];
            if(cookie == null || cookie.Value == null) return string.Empty;
            return cookie.Value;
        }

        private readonly static string RootPath = Webx.MapUrl("~/");
        public override void Set(string name, object value)
            => Set(name, Convert.ToString(value), null, false);

        public void Set(string name, string value, string path = null, bool httpOnly = false)
            => Set(name, value, DateTime.Now.AddDays(3), path, httpOnly);

        public void Set(string name, string value, DateTime expires, string path = null, bool httpOnly = false)
        {
            var ctx = HttpContext.Current;
            var response = ctx.Response;

            var cookie = response.Cookies[name];
            cookie.Value = value;
            cookie.Expires = value == null ? DateTime.Now.AddYears(-1) : expires;
            cookie.HttpOnly = httpOnly;
            cookie.Path = path ?? RootPath;
        }

        public override void Remove(string name)
        {
            this.Set(name, null);
        }
    }
}
