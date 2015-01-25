using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSample.Models;

namespace WebSample
{
    public abstract class QController : XControllerBase<User> { }
    public abstract class QViewPageBase : XWebViewPageBase<User> { }
    public abstract class QViewPageBase<TModel> : XWebViewPageBase<User, TModel> { }
}