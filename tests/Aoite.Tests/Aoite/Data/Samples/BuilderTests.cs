using System;
using Xunit;

namespace Aoite.Data.Builder
{

    public class BuilderTests
    {
        private readonly DbEngine engine = new DbEngine(new SqlEngineProvider("1"));

        class TestTable
        {
            public int ID { get; set; }
            public string UserName { get; set; }
        }

        [Fact()]
        public void CreateWhereTest()
        {
            var where = engine.CreateWhere(new ExecuteParameterCollection("id", 5));
            Assert.Equal("[id]=@id", where);
            where = DbExtensions.CreateWhere(engine, new ExecuteParameterCollection("id", 5, "un", "a"));
            Assert.Equal("[id]=@id AND [un]=@un", where);
        }

        [Fact()]
        public void WhereInTest1()
        {
            var command = engine.Select<TestTable>().WhereIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE [ID] IN (@id0, @id1, @id2)", command.Text);
        }

        [Fact()]
        public void WhereInTest2()
        {
            var command = engine.Select<TestTable>()
                .Where("UserName", "abc")
                .AndIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE [UserName]=@UserName AND [ID] IN (@id0, @id1, @id2)", command.Text);
        }


        [Fact()]
        public void WhereInTest3()
        {
            var command = engine.Select<TestTable>()
                .Where("@UserName", "abc")
                .AndNotIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE [UserName]=@UserName AND [ID] NOT IN (@id0, @id1, @id2)", command.Text);
        }

        [Fact()]
        public void SelectTest()
        {
            var command = engine.Select<TestTable>().End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable]", command.Text);
        }

        [Fact()]
        public void SelectByFieldsTest()
        {
            var command = engine.Select<TestTable>("Name").End();
            Assert.NotNull(command);
            Assert.Equal("SELECT Name FROM [TestTable]", command.Text);
        }

        [Fact()]
        public void SelectWhereTest()
        {
            var command = engine.Select<TestTable>()
                            .Where("ID=@id", "@id", 5)
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE ID=@id", command.Text);
            Assert.Equal(1, command.Count);
            Assert.Equal("@id", command[0].Name);
            Assert.Equal(5, command[0].Value);
        }

        [Fact()]
        public void SelectWhereGroupsTest()
        {
            var command = engine.Select<TestTable>()
                            .Where("ID=@id", "@id", 5)
                            .And("Name=@name", "@name", "abc")
                            .Or()
                                .BeginGroup("ID=@id2", "@id2", 6)
                                .Or("ID=@id3", "@id3", 7)
                                .EndGroup()
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE ID=@id AND Name=@name OR (ID=@id2 OR ID=@id3)", command.Text);
            Assert.Equal(4, command.Count);
        }

        [Fact()]
        public void SelectSomeWhere()
        {
            var command = engine.Select<TestTable>()
                            .Where("ID", "@id", new int[] { 1, 2, 3 })
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [TestTable] WHERE ([ID]=@id0 OR [ID]=@id1 OR [ID]=@id2)", command.Text);
            Assert.Equal(3, command.Count);
        }

#if !NET45 && !NET46
        [Fact()]
        public void Parse_FormattableString_To_ExecuteCommand_Test()
        {
            var engine = new DbEngine(new SqlEngineProvider("connection string"));
            var username = "daniel";
            var fields = "username,password,memo";
            var command = engine.Parse($"SELECT {fields::} FROM Users WHERE Username = {username}");
            Assert.Equal("SELECT username,password,memo FROM Users WHERE Username = @p0", command.Text);
            Assert.Equal(1, command.Count);
            Assert.Equal("@p0", command[0].Name);
            Assert.Equal(username, command[0].Value);
        }
#endif
    }
}
