using Xunit;

namespace Aoite.Data.Builder
{
    public class WhereCommandBuilderTests
    {
        private IWhere GetWhereSelect()
        {
            return new SqlBuilder(new DbEngine(new SqlEngineProvider("1"))).Where();
        }
        [Fact()]
        public void SqlTest()
        {
            var where = this.GetWhereSelect();
            where.Sql("a=b");
            Assert.Equal("SELECT * FROM  WHERE a=b", where.Text);
            where.Sql(" AND c=d");
            Assert.Equal("SELECT * FROM  WHERE a=b AND c=d", where.Text);
        }

        [Fact()]
        public void ParameterTest()
        {
            var where = this.GetWhereSelect();
            where.Parameter("@a", 1);
            where.Parameter("@b", 2);
            Assert.Equal(2, where.Parameters.Count);
        }

        [Fact()]
        public void AndTest()
        {
            var where = this.GetWhereSelect();
            where.And();
            Assert.Equal("SELECT * FROM ", where.Text);
            where.And("a=@a", "@a", 1)
                 .And("b=@a");
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@a", where.Text);
        }

        [Fact()]
        public void OrTest()
        {
            var where = this.GetWhereSelect();
            
            where.Or();
            Assert.Equal("SELECT * FROM ", where.Text);
            where.Or("a=@a", "@a", 1)
                 .Or("b=@a");
            Assert.Equal("SELECT * FROM  WHERE a=@a OR b=@a", where.Text);
        }

        [Fact()]
        public void BeginGroupTest()
        {
            var where = this.GetWhereSelect();
            
            where.BeginGroup();
            Assert.Equal("SELECT * FROM  WHERE (", where.Text);
        }

        [Fact()]
        public void BeginGroup_SSOTest()
        {
            var where = this.GetWhereSelect();
            
            where.BeginGroup("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE (a=@a", where.Text);
        }

        [Fact()]
        public void EndGroupTest()
        {
            var where = this.GetWhereSelect();
            
            where.EndGroup();
            Assert.Equal("SELECT * FROM  WHERE )", where.Text);
        }

        [Fact()]
        public void And_STest()
        {
            var where = this.GetWhereSelect();
            
            where.And("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", where.Text);
            where.And("b=1");
            Assert.Equal("SELECT * FROM  WHERE a=1 AND b=1", where.Text);
        }

        [Fact()]
        public void And_SSOTest()
        {
            var where = this.GetWhereSelect();
            
            where.And("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", where.Text);
            where.And("b=@b", "@b", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@b", where.Text);
        }

        [Fact()]
        public void And_SSTTest()
        {
            var where = this.GetWhereSelect();
            

            where.And("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE ([a]=@a0 OR [a]=@a1 OR [a]=@a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);

            where.And("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE ([a]=@a0 OR [a]=@a1 OR [a]=@a2) AND ([b]=@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }

        [Fact()]
        public void Or_STest()
        {
            var where = this.GetWhereSelect();
            
            where.Or("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", where.Text);
            where.Or("b=1");
            Assert.Equal("SELECT * FROM  WHERE a=1 OR b=1", where.Text);
        }

        [Fact()]
        public void Or_SSOTest()
        {
            var where = this.GetWhereSelect();
            
            where.Or("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", where.Text);
            where.Or("b=@b", "@b", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a OR b=@b", where.Text);
        }

        [Fact()]
        public void Or_SSTTest()
        {
            var where = this.GetWhereSelect();
            

            where.Or("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE ([a]=@a0 OR [a]=@a1 OR [a]=@a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);

            where.Or("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE ([a]=@a0 OR [a]=@a1 OR [a]=@a2) OR ([b]=@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }

        [Fact()]
        public void AndInTest()
        {
            var where = this.GetWhereSelect();
            

            where.AndIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE [a] IN (@a0, @a1, @a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);
            where.AndIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE [a] IN (@a0, @a1, @a2) AND [b] IN (@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }

        [Fact()]
        public void AndNotInTest()
        {
            var where = this.GetWhereSelect();
            
            where.AndNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE [a] NOT IN (@a0, @a1, @a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);
            where.AndNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE [a] NOT IN (@a0, @a1, @a2) AND [b] NOT IN (@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }

        [Fact()]
        public void OrInTest()
        {
            var where = this.GetWhereSelect();
            

            where.OrIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE [a] IN (@a0, @a1, @a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);
            where.OrIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE [a] IN (@a0, @a1, @a2) OR [b] IN (@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }

        [Fact()]
        public void OrNotInTest()
        {
            var where = this.GetWhereSelect();
            
            where.OrNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE [a] NOT IN (@a0, @a1, @a2)", where.Text);
            Assert.Equal(3, where.Parameters.Count);
            where.OrNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE [a] NOT IN (@a0, @a1, @a2) OR [b] NOT IN (@b0)", where.Text);
            Assert.Equal(4, where.Parameters.Count);
        }
    }
}
