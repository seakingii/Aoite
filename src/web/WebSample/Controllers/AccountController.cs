using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSample.ViewModels;
using WebSample.Arguments;
using WebSample.Models;

namespace WebSample.Controllers
{
    public class AccountController : QController
    {
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
                var user = this.Bus.FindOneWhere<User, LoggedUser>(args);
                if(user != null)
                {
                    user.LoginTime = DateTime.Now;
                    MvcClient.Identity = user;
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Model", "账号或密码错误。");
            }
            return View(args);
        }

        public ActionResult Logout()
        {
            MvcClient.Identity = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        public Result<GridData<User>> FindAll(UserFindAllArguments args)
        {
            return this.Execute(args.CopyTo<Commands.FindAllUser>()).ResultValue;
        }

        public bool Exists(string username, Guid? id = null)
        {
            if(string.IsNullOrEmpty(username)) return false;
            if(id.HasValue)
                return this.Bus.ExistsWhere<User>("Username=@username AND Id<>@id", new { username, Id = id.Value });
            return this.Bus.Exists<User>("Username", username);
        }

        [HttpPost]
        public Result Save(UserSaveArguments args)
        {
            if(!ModelState.IsValid) return ModelState.AllErrors();
            if(this.Exists(args.Username, args.Id)) return "用户名已存在。";

            var user = args.CopyTo<User>();
            if(!args.Id.HasValue)
            {
                user.Id = Guid.NewGuid();
                this.Bus.Add(user);
                return new Result<Guid>(user.Id);
            }
            else
            {
                this.Bus.Modify(user);
                return Result.Successfully;
            }

        }

        [HttpPost]
        public Result Remove(params Guid[] idLists)
        {
            this.Bus.RemoveAnonymous<User>(idLists);
            return Result.Successfully;
        }
    }
}