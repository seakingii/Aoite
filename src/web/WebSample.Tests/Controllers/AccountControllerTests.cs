using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using WebSample.Arguments;
using WebSample.Models;
using WebSample.ViewModels;
using Xunit;

namespace WebSample.Controllers
{
    public class AccountControllerTests
    {
        [Fact()]
        public void Get_Login_Test()
        {
            var c = WebConfig.Mocks.Create<AccountController>();
            c.Login().AssertViewName().AssertModel(new LoginArguments());
        }

        [Fact()]
        public void Post_Login_Test_Success()
        {
            var loggedUser = new LoggedUser()
            {
                Id = Guid.NewGuid(),
                LoginTime = DateTime.Now,
                Username = "test",
                RealName = "admin",
            };

            var c = WebConfig.Mocks.Create<AccountController>(f => f
                .Mock<CMD.FindOneWhere<User, LoggedUser>>((context, command) =>
                {
                    Assert.Equal("test", command.WhereParameters.Parameters["Username"].Value);
                    Assert.Equal("123456", command.WhereParameters.Parameters["Password"].Value);
                    command.ResultValue = loggedUser;
                }));

            c.Login(new LoginArguments()
            {
                Username = "test",
                Password = "123456"
            }).AssertRoute("Index", "Home");

            Assert.Equal(loggedUser, c.User);
        }

        [Fact()]
        public void Post_Login_Test_Faild()
        {
            var c = WebConfig.Mocks.Create<AccountController>(f => f
                .Mock<CMD.FindOneWhere<User, LoggedUser>>((context, command) =>
                {
                    Assert.Equal("test", command.WhereParameters.Parameters["Username"].Value);
                    Assert.Equal("123456", command.WhereParameters.Parameters["Password"].Value);
                    command.ResultValue = null;
                }));
            c.Login(new LoginArguments()
            {
                Username = "test",
                Password = "123456"
            }).AssertViewName();

            c.AssertModelState("Model", "账号或密码错误。");
            Assert.Null(c.User);
        }

        [Fact()]
        public void Get_Logout_Test()
        {
            var c = WebConfig.Mocks.Create<AccountController>(new LoggedUser() { Username = "张三" });
            Assert.Equal("张三", c.User.Username);
            c.Logout().AssertRoute("Login");
            Assert.Null(c.User);
        }

        [Fact()]
        public void Post_FindAll_Test()
        {
            var c = WebConfig.Mocks.Create<AccountController>(f => f
                .Mock<Commands.FindAllUser>((context, command) =>
                {
                    Assert.Equal(2, command.PageNumber);
                    Assert.Equal(5, command.PageSize);
                    Assert.Equal("a", command.LikeRealName);
                    Assert.Equal("b", command.LikeUsername);
                    command.ResultValue = new GridData<User>()
                    {
                        Rows = new User[1] { new User { Username = "x" } },
                        Total = 1
                    };
                }));

            new GridData<User>()
            {
                Rows = new User[1] { new User { Username = "x" } },
                Total = 1
            }.AssertObject(c.FindAll(new UserFindAllArguments()
            {
                PageNumber = 2,
                PageSize = 5,
                LikeRealName = "a",
                LikeUsername = "b",
            }));

        }

        [Fact()]
        public void Get_Exists_Test()
        {
            var id = Guid.NewGuid();
            var c = WebConfig.Mocks.Create<AccountController>(f => f
               .Mock<CMD.ExistsWhere<User>>((context, command) =>
               {
                   Assert.Equal("abc", command.WhereParameters.Parameters["Username"].Value);
                   Assert.Equal(id, command.WhereParameters.Parameters["Id"].Value);
               })
               .Mock<CMD.Exists<User>>((context, command) =>
               {
                   Assert.Equal("Username", command.KeyName);
                   Assert.Equal("abc", command.KeyValue);
               }));
            Assert.False(c.Exists("abc"));
            Assert.False(c.Exists("abc", id));
        }

        [Fact()]
        public void Post_Save_Test()
        {
            var isExists = true;
            var args = new UserSaveArguments()
            {
                Id = Guid.NewGuid(),
                Username = "abc",
                Password = "123",
                RealName = "test",
            };

            var c = WebConfig.Mocks.Create<AccountController>(f => f
               .Mock<CMD.ExistsWhere<User>>((context, command) =>
               {
                   Assert.Equal("abc", command.WhereParameters.Parameters["Username"].Value);
                   Assert.Equal(args.Id.Value, command.WhereParameters.Parameters["Id"].Value);
                   command.ResultValue = isExists;
               })
               .Mock<CMD.Exists<User>>((context, command) =>
               {
                   Assert.Equal("Username", command.KeyName);
                   Assert.Equal("abc", command.KeyValue);
                   command.ResultValue = isExists;
               })
               .Mock<CMD.Modify<User>>((context, command) =>
               {
                   command.Entity.AssertObject(args.CopyTo<User>());
                   command.ResultValue = 1;
               })
               .Mock<CMD.Add<User>>((context, command) =>
               {
                   command.Entity.AssertObject(args.CopyTo<User>());
                   command.ResultValue = 1;
               }));
            Assert.Equal("用户名已存在。", c.Save(args).Message);
            args.Id = null;
            Assert.Equal("用户名已存在。", c.Save(args).Message);
            args.Id = Guid.NewGuid();
            isExists = false;
            c.Save(args).ThrowIfFailded();
            args.Id = null;
            Assert.NotNull((c.Save(args) as Result<Guid>));
        }

        [Fact()]
        public void Post_Remove_Test()
        {
            var id = Guid.NewGuid();
            var c = WebConfig.Mocks.Create<AccountController>(f => f
               .Mock<CMD.Remove<User>>((context, command) =>
               {
                   Assert.Equal(new Guid[] { id }, command.Entity);
                   command.ResultValue = 1;
               }));
            c.Remove(id).ThrowIfFailded();
        }
    }
}
