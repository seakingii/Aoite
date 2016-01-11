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
            var engine = new DbEngine(new SqliteEngineProvider(dbFile, null));
            if(!engine.Execute("SELECT 1 FROM sqlite_master WHERE type='table' AND name='SampleUser'").ToScalar<bool>())
            {
                engine.Execute("CREATE TABLE SampleUser(Id INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,Username nvarchar(100) NOT NULL,Password varchar(100) NOT NULL)")
                      .ToNonQuery();
            }

            var rowCount = engine.RowCount<SampleUser>();

            Console.WriteLine("Table<SampleUser> RowCount => {0};", rowCount);

            if(rowCount == 0)
            {
                Console.WriteLine("Create admin(123456) user;");
                using(var context = engine.Context)
                {
                    context.Add(new SampleUser() { Username = "admin", Password = "123456" });
                    Console.WriteLine("The admin Id is {0};", context.GetLastIdentity<SampleUser>());
                }
            }


            Console.ReadLine();
        }
    }


}
