using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Data.Samples
{
    public class SimpleDataTests
    {
        private TestManagerBase CreateManager()
        {
            return new MsCeTestManager();
            //return new MsSqlTestManager("Data Source=localhost;Initial Catalog=master;Integrated Security=True;");
        }

        private void CreateTable(TestManagerBase manager)
        {
            var sql = ("CREATE TABLE TestTable(ID bigint PRIMARY KEY identity(1,1),UserName nvarchar(255))");
            manager.Engine
                .Execute(sql)
                .ToNonQuery();

        }
        private void InsertRows(TestManagerBase manager, int rowCount = 10)
        {
            using(var dbContext = manager.Engine.ContextTransaction)
            {
                for(int i = 0; i < rowCount; i++)
                {
                    dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                        .ToNonQuery();
                }
                dbContext.Commit();
            }
        }

        [Fact()]
        public void ToNonQueryTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                using(var dbContext = manager.Engine.ContextTransaction)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i).ToNonQuery();
                    }
                    dbContext.Commit();

                    Assert.Equal(10, dbContext.Execute("SELECT COUNT(*) FROM TestTable").ToScalar<int>());
                }
            }
        }

        [Fact()]
        public void ToScalarTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Assert.Equal((object)0
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar()
                        );


                Assert.Equal(0
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar<int>()
                        );

                Assert.Equal(0L
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar<long>()
                        );
            }
        }
        [Fact()]
        public void TransactionTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                Assert.Equal(10
                       , manager.Engine
                           .Execute("SELECT COUNT(*) FROM TestTable")
                           .ToScalar<int>()
                       );
                using(var dbContext = manager.Engine.ContextTransaction)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                            .ToNonQuery();
                        if(i == 4)
                        {
                            dbContext.Rollback();
                            dbContext.OpenTransaction();
                        }
                    }
                    dbContext.Commit();
                }
                Assert.Equal(15
               , manager.Engine
                   .Execute("SELECT COUNT(*) FROM TestTable")
                   .ToScalar<int>()
               );
            }
        }

        private void InsertPerformanceTest(IDbEngine engine)
        {
            for(int i = 0; i < 10000; i++)
            {
                engine.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                    .ToNonQuery();
            }
        }

        private void InsertCountTest(IDbEngine engine)
        {
            Assert.Equal(10000
                     , engine
                         .Execute("SELECT COUNT(*) FROM TestTable")
                         .ToScalar<int>()
                     );
        }

#if TEST_MSSQL
        [Fact()]
        public void InsertPerformanceTest_Ado()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                using(var context = manager.Engine.Context)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    var conn = manager.Engine.CreateConnection();
                    conn.Open();
                    for(int i = 0; i < 10000; i++)
                    {
                        var command = conn.CreateCommand();
                        command.CommandText = "INSERT INTO TestTable(UserName) VALUES (@username)";
                        var p = command.CreateParameter();
                        p.ParameterName = "@username";
                        p.Value = "user" + i;
                        command.Parameters.Add(p);
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                    watch.Stop();
                    Console.WriteLine(watch.Elapsed);
                    InsertCountTest(manager.Engine);
                }
                //if(elapsed.TotalMilliseconds< 1000)
            }
        }

        [Fact()]
        public void InsertPerformanceTest_Engine()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                InsertPerformanceTest(manager.Engine);
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
            }
        }

        [Fact()]
        public void InsertPerformanceTest_Context()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using(var context = manager.Engine.Context)
                {
                    InsertPerformanceTest(context);
                }

                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
            }
        }

        [Fact()]
        public void InsertPerformanceTest_ContextT()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using(var context = manager.Engine.ContextTransaction)
                {
                    InsertPerformanceTest(context);
                    context.Commit();
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
                //if(elapsed.TotalMilliseconds< 1000)
            }
        }
#endif
        [Fact()]
        public void ToTableTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToTable();
                Assert.Equal(1, r.Rows.Count);
                Assert.Equal(r.Rows[0][1], "user0");
            }
        }
        [Fact()]
        public void ToTablePageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                using(var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToTable(pageNumber, pageSize))
                {

                    Assert.Equal(pageSize, r.Rows.Count);
                    Assert.Equal(rowCount, r.Total);
                    for(int i = 0; i < pageSize; i++)
                    {
                        Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r.Rows[i][1]);
                    }
                }
            }
        }

        class TestTable
        {
            public long ID { get; set; }
            public string UserName { get; set; }
        }
        class NameTable
        {
            public string UserName { get; set; }
        }

        [Fact()]
        public void ToEntityTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntity<TestTable>()
                    ;
                Assert.Equal(r.UserName, "user0");
            }
        }


        [Fact()]
        public void ToEntitiesTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntities<TestTable>()
                    ;
                Assert.Equal(1, r.Count);
                Assert.Equal(r[0].UserName, "user0");
            }
        }

        [Fact()]
        public void ToEntitiesPageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                     .ToEntities<TestTable>(pageNumber, pageSize);


                Assert.Equal(pageSize, r.Rows.Length);
                Assert.Equal(rowCount, r.Total);
                for(int i = 0; i < pageSize; i++)
                {
                    Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r[i].UserName);
                }

            }
        }

        [Fact()]
        public void ToDynamicEntityTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntity()
                    ;
                Assert.Equal(r.Id, 1L);
                Assert.Equal(r.username, "user0");
            }
        }

        [Fact()]
        public void ToDynamicEntitiesTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntities()
                    ;
                Assert.Equal(1, r.Count);
                Assert.Equal(r[0].UserName, "user0");
            }
        }

        [Fact()]
        public void ToDynamicEntitiesPageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToEntities(pageNumber, pageSize);

                Assert.Equal(pageSize, r.Rows.Length);
                Assert.Equal(rowCount, r.Total);
                for(int i = 0; i < pageSize; i++)
                {
                    Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r[i].UserName);
                }
            }
        }
        [Fact()]
        public void CalrTest()
        {
            /* PS：两倍性能损耗，非常棒！*/
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                for(int i = 0; i < 100; i++)
                {
                    var r = manager.Engine
                        .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                        .ToEntity<TestTable>()
                        ;
                    Assert.Equal(r.ID, 1L);
                    Assert.Equal(r.UserName, "user0");
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed);


            watch.Start();
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                for(int i = 0; i < 100; i++)
                {
                    var r = manager.Engine
                        .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                        .ToEntity()
                        ;
                    Assert.Equal(r.Id, 1L);
                    Assert.Equal(r.username, "user0");
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }


        [Fact()]
        public void FineOneTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine.FindOne<TestTable>(5);
                Assert.NotNull(r);
                var r2 = manager.Engine.FindOne<TestTable, NameTable>(5);
                Assert.NotNull(r2);
            }
        }

        [Fact()]
        public void FineOneWhereTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r2 = manager.Engine.FindOneWhere<TestTable, NameTable>(new { username = "user4", id = 5 });
                Assert.NotNull(r2);
            }
        }
        [Fact()]
        public void FineAllWhereTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r2 = manager.Engine.FindAllWhere<TestTable, NameTable>("id<=@uid", new ExecuteParameterCollection("@uid", 5));
                Assert.Equal(5, r2.Count);
            }
        }

#if !NET40
        [Fact()]
        public async void ToNonQueryAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                using(var dbContext = manager.Engine.ContextTransaction)
                {
                    List<Task> tasks = new List<Task>();
                    for(int i = 0; i < 10; i++)
                    {
                        tasks.Add(dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i).ToNonQueryAsync());
                    }
                    Task.WaitAll(tasks.ToArray());
                    dbContext.Commit();

                    Assert.Equal(10, await dbContext.Execute("SELECT COUNT(*) FROM TestTable").ToScalarAsync<int>());
                }
            }
        }

        [Fact()]
        public async void ToScalarAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Assert.Equal((object)0
                        , await manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalarAsync()
                        );


                Assert.Equal(0
                        , await manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalarAsync<int>()
                        );

                Assert.Equal(0L
                        , await manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalarAsync<long>()
                        );
            }
        }

        [Fact()]
        public async void ToTableAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = await manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToTableAsync();
                Assert.Equal(1, r.Rows.Count);
                Assert.Equal(r.Rows[0][1], "user0");
            }
        }

        [Fact()]
        public async void ToTablePageAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                using(var r = await manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToTableAsync(pageNumber, pageSize))
                {

                    Assert.Equal(pageSize, r.Rows.Count);
                    Assert.Equal(rowCount, r.Total);
                    for(int i = 0; i < pageSize; i++)
                    {
                        Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r.Rows[i][1]);
                    }
                }
            }
        }

        [Fact()]
        public async void ToEntityAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = await manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntityAsync<TestTable>()
                    ;
                Assert.Equal(r.UserName, "user0");
            }
        }

        [Fact()]
        public async void ToEntitiesAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = await manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntitiesAsync<TestTable>()
                    ;
                Assert.Equal(1, r.Count);
                Assert.Equal(r[0].UserName, "user0");
            }
        }
        [Fact()]
        public async void ToEntitiesPageAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                var r = await manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                     .ToEntitiesAsync<TestTable>(pageNumber, pageSize);


                Assert.Equal(pageSize, r.Rows.Length);
                Assert.Equal(rowCount, r.Total);
                for(int i = 0; i < pageSize; i++)
                {
                    Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r[i].UserName);
                }

            }
        }
        [Fact()]
        public async void ToDynamicEntityAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = await manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntityAsync()
                    ;
                Assert.Equal(r.Id, 1L);
                Assert.Equal(r.username, "user0");
            }
        }

        [Fact()]
        public async void ToDynamicEntitiesAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = await manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntitiesAsync()
                    ;
                Assert.Equal(1, r.Count);
                Assert.Equal(r[0].UserName, "user0");
            }
        }

        [Fact()]
        public async void ToDynamicEntitiesPageAsyncTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                var r = await manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToEntitiesAsync(pageNumber, pageSize);

                Assert.Equal(pageSize, r.Rows.Length);
                Assert.Equal(rowCount, r.Total);
                for(int i = 0; i < pageSize; i++)
                {
                    Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r[i].UserName);
                }
            }
        }
#endif
    }
}
