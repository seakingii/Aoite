using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSample.Models;

namespace WebSample.Controllers
{
    public class MemberController : QController
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginArguments());
        }

        [AllowAnonymous, HttpPost]
        public ActionResult Login(LoginArguments args)
        {
            if(ModelState.IsValid)
            {
                if(args.Password == "123456")
                {
                    MvcClient.Identity = new User() { Username = args.Username, LoginTime = DateTime.Now };
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Model", "账号或密码错误");
            }
            return View(args);
        }
    }
}