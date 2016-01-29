using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Data.Samples
{
    public class ValidatorAttributeTests
    {

        class TestTable
        {
            public long Id { get; set; }
            [StringLength(10, true)]
            public string Username { get; set; }
            [StringLength(10, false), NotNull]
            public string Password { get; set; }
        }

        [Fact]
        public void StringLength_Test()
        {
            var engine = DbEngine.Create("sql", "1");

            Assert.Throws<ArgumentNullException>(() => engine.Add(new TestTable()
            {
                Id = 5
            }));


            var ex1 = Assert.Throws<ArgumentException>(() => engine.Add(new TestTable()
            {
                Id = 5,
                Password = "一二三四五六七八九十",
                Username = "一二三四五六七八九十"
            }));

            Assert.Equal("Password", ex1.ParamName);


            var ex2 = Assert.Throws<ArgumentException>(() => engine.Add(new TestTable()
            {
                Id = 5,
                Password = "一二三四五",
                Username = "一二三四五六七八九十1"
            }));

            Assert.Equal("Username", ex2.ParamName);
        }
    }
}
