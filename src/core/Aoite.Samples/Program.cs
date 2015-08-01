using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aoite.Data;

namespace Aoite.Samples
{
    class Program
    {
        class TestTable
        {
            [Column(true)]
            public long Id { get; set; }
            public string UserName { get; set; }
        }

        static void Sample01()
        {
            using(MsCeTestManager testManager = new MsCeTestManager())
            {
                //var engine = new MsSqlEngine("连接字符串");
                var engine = testManager.Engine;
                var result = engine.Execute("CREATE TABLE TestTable(ID bigint PRIMARY KEY identity(1,1),UserName nvarchar(255))")
                                   .ToNonQuery();
                if(result.IsFailed)
                {
                    Console.WriteLine(result.Exception);
                    return;
                }
                using(var dbContext = engine.ContextTransaction)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)"
                            , "@username", "user" + i)
                            .ToNonQuery()
                            .ThrowIfFailded();
                    }
                    dbContext.Commit();
                    /*
                     * - 1、手工回滚事务：dbContext.Rollback()
                     * - 2、自动回滚实物：没有调用 dbContext.Commit() 方法
                     */
                }
                engine.Add(new TestTable() { UserName = "user11" })
                      .ThrowIfFailded();
                engine.AddAnonymous<TestTable>(new { UserName = "user12" })
                      .ThrowIfFailded();

                var rowCount1 = engine.Execute("SELECT COUNT(*) FROM TestTable").ToScalar<long>().UnsafeValue;
                var rowCount2 = engine.RowCount<TestTable>().UnsafeValue;

                Console.WriteLine("总行数A {0}，总行数B {1}", rowCount1, rowCount2);

                engine.ModifyAnonymous<TestTable>(new { Id = 5, UserName = "Changed!" })
                      .ThrowIfFailded();

                var model = engine.FindOne<TestTable>(5).UnsafeValue;

                Console.WriteLine(model.UserName);

            }
        }

        static void Main(string[] args)
        {
            Sample01();
            Console.ReadLine();
        }
    }
}
