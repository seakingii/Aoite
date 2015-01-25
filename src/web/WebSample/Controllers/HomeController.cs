using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSample.Controllers
{
    [AllowAnonymous]
    public class HomeController : QController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}