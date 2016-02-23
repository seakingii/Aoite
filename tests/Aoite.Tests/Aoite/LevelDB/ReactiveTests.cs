using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.LevelDB.Tests
{
    public class ReactiveTests : IDisposable
    {
        public ReactiveTests()
        {
            var tempPath = Path.GetTempPath();
            var randName = Path.GetRandomFileName();
            DatabasePath = Path.Combine(tempPath, randName);
        }
        string CleanTestDB()
        {
            return DatabasePath;
        }
        string DatabasePath { get; set; }
        public void Dispose()
        {
            // some test-cases tear-down them self
            if(Directory.Exists(DatabasePath))
            {
                Directory.Delete(DatabasePath, true);
            }
        }
        [Fact]
        public void TestOpen()
        {
            var path = CleanTestDB();
            Assert.Throws<LevelDBException>(() =>
            {
                using(var db = new LDB(path, new Options { CreateIfMissing = true }))
                {
                }

                using(var db = new LDB(path, new Options { ErrorIfExists = true }))
                {
                }
            });
        }

        [Fact]
        public void TestCRUD()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Set("Tampa", "green");
                db.Set("London", "red");
                db.Set("New York", "blue");

                Assert.Equal((string)db.Get("Tampa"), "green");
                Assert.Equal((string)db.Get("London"), "red");
                Assert.Equal((string)db.Get("New York"), "blue");

                db.Remove("New York");

                Assert.Null(db.Get("New York"));

                db.Remove("New York");
            }
        }

        [Fact]
        public void TestRepair()
        {
            TestCRUD();
            LDB.Repair(DatabasePath, new Options());
        }

        [Fact]
        public void TestIterator()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Set("Tampa", "green");
                db.Set("London", "red");
                db.Set("New York", "blue");

                var expected = new[] { "London", "New York", "Tampa" };

                var actual = new List<string>();
                using(var iterator = db.CreateIterator(new ReadOptions()))
                {
                    iterator.SeekToFirst();
                    while(iterator.IsValid())
                    {
                        var key = iterator.GetKey();
                        actual.Add(key);
                        iterator.Next();
                    }
                }

                Assert.Equal(expected, actual);

            }
        }

        [Fact]
        public void TestEnumerable()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Set("Tampa", "green");
                db.Set("London", "red");
                db.Set("New York", "blue");

                var expected = new[] { "London", "New York", "Tampa" };
                var actual = from kv in db
                             select (string)kv.Key;

                Assert.Equal(expected, actual.ToArray());
            }
        }

        [Fact]
        public void TestSnapshot()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Set("Tampa", "green");
                db.Set("London", "red");
                db.Remove("New York");

                using(var snapShot = db.CreateSnapshot())
                {
                    var readOptions = new ReadOptions { Snapshot = snapShot };

                    db.Set("New York", "blue");

                    Assert.Equal((string)db.Get("Tampa", readOptions), "green");
                    Assert.Equal((string)db.Get("London", readOptions), "red");

                    // Snapshot taken before key was updates
                    Assert.Null(db.Get("New York", readOptions));
                }

                // can see the change now
                Assert.Equal((string)db.Get("New York"), "blue");

            }
        }

        [Fact]
        public void TestGetProperty()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                var r = new Random(0);
                var data = "";
                for(var i = 0; i < 1024; i++)
                {
                    data += 'a' + r.Next(26);
                }

                for(int i = 0; i < 5 * 1024; i++)
                {
                    db.Set(string.Format("row{0}", i), data);
                }

                var stats = db.PropertyValue("leveldb.stats");

                Assert.NotNull(stats);
                Assert.True(stats.Contains("Compactions"));
            }
        }

        [Fact]
        public void TestWriteBatch()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Set("NA", "Na");

                using(var batch = new WriteBatch())
                {
                    batch.Remove("NA")
                         .Set("Tampa", "Green")
                         .Set("London", "red")
                         .Set("New York", "blue");
                    db.Write(batch);
                }

                var expected = new[] { "London", "New York", "Tampa" };
                var actual = from kv in db
                             select (string)kv.Key;

                Assert.Equal(expected, actual.ToArray());
            }
        }
    }
}
