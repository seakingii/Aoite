using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSample.ViewModels;

namespace WebSample
{
    public abstract class QController : XControllerBase<LoggedUser> { }
    public abstract class QViewPageBase : XWebViewPageBase<LoggedUser> { }
    public abstract class QViewPageBase<TModel> : XWebViewPageBase<LoggedUser, TModel> { }
}