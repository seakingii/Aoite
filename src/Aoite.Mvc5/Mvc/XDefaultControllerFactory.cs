﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示基于 <see cref="Webx.Container"/> 已注册的控制器工厂。
    /// </summary>
    public class XDefaultControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// 初始化 <see cref="XDefaultControllerFactory"/> 类的新实例。
        /// </summary>
        public XDefaultControllerFactory() { }

        /// <summary>
        /// 检索指定请求上下文和控制器类型的控制器实例。
        /// </summary>
        /// <param name="requestContext">HTTP 请求的上下文，其中包括 HTTP 上下文和路由数据。</param>
        /// <param name="controllerType">控制器的类型。</param>
        /// <returns>控制器实例。</returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if(controllerType != null) return ObjectFactory.Context.Get(controllerType) as IController;
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}
