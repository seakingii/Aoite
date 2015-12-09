using Xunit;

namespace Aoite.Data.Builder
{
    public class SelectCommandBuilderTests
    {
        private ISelect GetSelect()
        {
            return new SqlBuilder(new DbEngine(new SqlEngineProvider("1")));
        }
        [Fact()]
        public void SelectTest()
        {
            var builder = this.GetSelect();
            builder.Select("a, b");
            Assert.Equal("SELECT a, b FROM ", builder.Text);
            builder.Select("c", "d");
            Assert.Equal("SELECT a, b, c, d FROM ", builder.Text);
        }

        [Fact()]
        public void FromTest()
        {
            var builder = this.GetSelect();
            builder.From("a");
            Assert.Equal("SELECT * FROM a", builder.Text);
            builder.Select("c", "d");
            Assert.Equal("SELECT c, d FROM a", builder.Text);
            builder.From("b");
            Assert.Equal("SELECT c, d FROM b", builder.Text);
        }

        [Fact()]
        public void OrderByTest()
        {
            var builder = this.GetSelect();
            builder.OrderBy("a");
            Assert.Equal("SELECT * FROM  ORDER BY a", builder.Text);
            builder.OrderBy("b");
            Assert.Equal("SELECT * FROM  ORDER BY a, b", builder.Text);
        }

        [Fact()]
        public void GroupByTest()
        {
            var builder = this.GetSelect();
            builder.GroupBy("a");
            Assert.Equal("SELECT * FROM  GROUP BY a", builder.Text);
            builder.GroupBy("b");
            Assert.Equal("SELECT * FROM  GROUP BY a, b", builder.Text);
        }

        [Fact()]
        public void WhereTest()
        {
            var builder = this.GetSelect();
            builder.Where();
            Assert.Equal("SELECT * FROM ", builder.Text);
            builder.Where("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.Text);
        }

        [Fact()]
        public void Where_STest()
        {
            var builder = this.GetSelect();
            builder.Where("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.Text);
            builder.Where("b=2");
            Assert.Equal("SELECT * FROM  WHERE a=1 AND b=2", builder.Text);
        }

        [Fact()]
        public void Where_SSTTest()
        {
            var builder = this.GetSelect();
            builder.Where("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2)", builder.Text);
            Assert.Equal(3, builder.Parameters.Count);

            builder.Where("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2) AND (b=@b0)", builder.Text);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void Where_SSOTest()
        {
            var builder = this.GetSelect();
            builder.Where("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", builder.Text);
            Assert.Equal(1, builder.Parameters.Count);
            builder.Where("b=@b", "@b", 2);
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@b", builder.Text);
            Assert.Equal(2, builder.Parameters.Count);
        }

        [Fact()]
        public void WhereInTest()
        {
            var builder = this.GetSelect();
            builder.WhereIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2)", builder.Text);
            Assert.Equal(3, builder.Parameters.Count);
            builder.WhereIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2) AND b IN (@b0)", builder.Text);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void WhereNotInTest()
        {
            var builder = this.GetSelect();
            builder.WhereNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2)", builder.Text);
            Assert.Equal(3, builder.Parameters.Count);
            builder.WhereNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2) AND b NOT IN (@b0)", builder.Text);
            Assert.Equal(4, builder.Parameters.Count);
        }
    }
}
