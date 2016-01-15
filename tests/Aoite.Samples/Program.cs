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
                    context.Add(new SampleUser() { Username = "admin", Password = "123456" });
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
                if(loginUserName != null) Console.Write($"Current user is {loginUserName}.");
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
                    default:
                        Console.WriteLine($"Invalid command {cmd}");
                        break;
                }
                GA.ResetContexts();//close db context or redis context or log context
                Console.WriteLine();
            } while(true);

        }
    }


}
