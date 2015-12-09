using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Data
{
    public class ExecuteCommandTests
    {
        [Fact]
        public void ToStringTest()
        {
            var command = new ExecuteCommand("select * from users where id=@userid and name=@name", "@userid", 1,"@name","张三");
            Assert.Equal(@"select * from users where id=@userid and name=@name
{
    @userid = 1
    @name = '张三'
}", command.ToString());
        }
    }
}
