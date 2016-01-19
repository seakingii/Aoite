using Aoite.CommandModel;
using Aoite.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbFile = GA.FullPath("Samples.db");
            var engine = new DbEngine(new SQLiteEngineProvider(dbFile, null));

            //- Test table is existed.
            if(!engine.Execute("SELECT 1 FROM sqlite_master WHERE type='table' AND name='SampleUser'").ToScalar<bool>())
            {
                //- Create SQLite table.
                engine.Execute("CREATE TABLE SampleUser(Id INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,Username nvarchar(100) NOT NULL,Password varchar(100) NOT NULL)")
                      .ToNonQuery();
            }

            //- Create db provider with SQLite.
            engine = DbEngine.Create("sqlite", "Data Source=" + dbFile + ";Version=3;");

            //- Create admin user.
            if(engine.RowCount<SampleUser>() == 0)
            {
                Console.WriteLine("Create admin(123456) user;");
                using(var context = engine.Context)
                {
                    context.Add(new SampleUser() { Username = "admin", Password = "admin" });
                    Console.WriteLine("The user admin Id is {0}.", context.GetLastIdentity<SampleUser>());
                }
            }

            string loginUserName = null;

            //- Create a ioc container.
            var container = new IocContainer()
                .AddService<IUserFactory>(new UserFactory(c => loginUserName))//- Get current login user callback.
                .AddService<IDbEngine>(lmps => engine.Context);//- Get or create a new current thread db context.

            var userService = new SampleUserService();
            userService.Container = container;//- Set service's container.
            do
            {
                if(loginUserName != null) Console.WriteLine($"Current user is {loginUserName}.");
                Console.Write("Please input a command:");

                var cmd = Console.ReadLine();
                switch(cmd.ToLower())
                {
                    case "login":
                        {
                            Console.Write("Please input login username:");
                            var username = Console.ReadLine();
                            Console.Write("Please input login password:");
                            var password = Console.ReadLine();
                            if(!userService.Check(username, password))
                            {
                                Console.WriteLine("Invalid username or password.");
                                break;
                            }
                            loginUserName = username;
                            Console.WriteLine($"{loginUserName} logined.");
                        }
                        break;
                    case "logout":
                        Console.WriteLine($"{loginUserName} logouted.");
                        loginUserName = null;
                        break;
                    case "modifypassword":
                        {
                            Console.Write("Please input a new password:");
                            var newPassword = Console.ReadLine();
                            if(userService.ModifyPassowrd(newPassword)) Console.WriteLine("Modify password succeed.");
                            else Console.WriteLine("Please login.");
                        }
                        break;
                    case "add":
                        {
                            Console.Write("Please input a new username:");
                            var username = Console.ReadLine();
                            Console.Write("Please input a new password:");
                            var password = Console.ReadLine();
                            if(!userService.Add(username, password))
                            {
                                if(loginUserName != "admin") Console.WriteLine("The current user is not admin.");
                                else Console.WriteLine($"Username \"{username}\" is existed");
                                break;
                            }
                            Console.WriteLine($"{username} is added.");
                        }
                        break;
                    case "remove":
                        {
                            Console.Write("Please input the remove username:");
                            var username = Console.ReadLine();
                            if(!userService.Remove(username))
                            {
                                if(loginUserName != "admin") Console.WriteLine("The current user is not admin.");
                                else Console.WriteLine($"Username \"{username}\" is not existed");
                                break;
                            }
                            Console.WriteLine($"{username} is removed.");
                        }
                        break;
                    case "count":
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("mm:ss")} : currently {userService.Count()} users.");
                        }
                        break;
                    case "list":
                        {
                            var column = string.Format("|{0,5}|{1,10}|{2,10}|", "Id", "Username", "Password");
                            Console.WriteLine();
                            Console.WriteLine(column);
                            Console.WriteLine(new string('-', column.Length));
                            var list = userService.GetList();
                            foreach(var item in list)
                            {
                                Console.WriteLine("|{0,5}|{1,10}|{2,10}|", item.Id, item.Username, item.Password);
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"Invalid command {cmd}");
                        break;
                }
                engine.ResetContext();//close db context
                Console.WriteLine();
            } while(true);

        }
    }


}
