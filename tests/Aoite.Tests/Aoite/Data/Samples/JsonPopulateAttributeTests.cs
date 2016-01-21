using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Data.Samples
{
    public class JsonPopulateAttributeTests
    {
        private TestManagerBase CreateManager()
        {
            return new MsCeTestManager();
            //return new MsSqlTestManager("Data Source=localhost;Initial Catalog=master;Integrated Security=True;");
        }

        private void CreateTable(TestManagerBase manager)
        {
            var sql = ("CREATE TABLE TestTable(Id bigint PRIMARY KEY identity(1,1),UserName nvarchar(255),Roles nvarchar(2000))");
            manager.Engine
                .Execute(sql)
                .ToNonQuery();

        }
        class TestTable
        {
            public long Id { get; set; }
            public string Username { get; set; }
            [JsonColumn]
            public List<TestRole> Roles { get; set; }
        }
        class TestRole
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
        [Fact()]
        public void AddTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                var engine = manager.Engine;
                engine.Add(new TestTable()
                {
                    Username = "test",
                    Roles = new List<TestRole>()
                    {
                        new TestRole() { Id=1, Name="Role1"},
                        new TestRole() { Id=2, Name="Role2"},
                        new TestRole() { Id=3, Name="Role3"},
                    }
                });

                var item = engine.FindOne<TestTable>(1);
                Assert.Equal("test", item.Username);
                Assert.Equal(3, item.Roles.Count);
                var array = item.Roles.OrderBy(r => r.Id).ToArray();
                Assert.Equal(2, array[1].Id);
                Assert.Equal("Role2", array[1].Name);
                var json = engine.Execute("SELECT Roles FROM TestTable").ToScalar<string>();
                //Assert.Equal("[{\"Id\":1,\"Name\":\"Role1\"},{\"Id\":2,\"Name\":\"Role2\"},{\"Id\":3,\"Name\":\"Role3\"}]", json);

                Assert.Equal("[{\"id\":1,\"name\":\"Role1\"},{\"id\":2,\"name\":\"Role2\"},{\"id\":3,\"name\":\"Role3\"}]", json);
            }
        }
    }
}
