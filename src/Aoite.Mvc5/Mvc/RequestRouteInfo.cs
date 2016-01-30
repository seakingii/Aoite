using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示请求的路由信息。
    /// </summary>
    public class RequestRouteInfo
    {
        private readonly RouteData _routeData;
        /// <summary>
        /// 初始化一个 <see cref="RequestRouteInfo"/> 类的新实例。
        /// </summary>
        /// <param name="routeData"></param>
        public RequestRouteInfo(RouteData routeData)
        {
            if(routeData == null) throw new ArgumentNullException(nameof(routeData));
            this._routeData = routeData;
        }

        /// <summary>
        /// 指定参数名称，获取当前请求的参数值。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>参数值。</returns>
        public object Object(string name) => this._routeData.Values[name];
        /// <summary>
        /// 指定参数名称，获取当前请求的参数值（字符串形式）。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>字符串形式的参数值。</returns>
        public string String(string name) => Convert.ToString(Object(name));
        /// <summary>
        /// 获取当前请求的区域（Area）。
        /// </summary>
        public string Area => Convert.ToString(this._routeData.DataTokens["area"]);
        /// <summary>
        /// 获取当前请求的操作（Action）。
        /// </summary>
        public string Controller => String("controller");
        /// <summary>
        /// 获取当前请求的操作（Action）。
        /// </summary>
        public string Action => String("action");
    }
}
