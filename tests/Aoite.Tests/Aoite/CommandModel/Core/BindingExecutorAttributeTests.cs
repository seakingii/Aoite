using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.CommandModel
{
    public class BindingExecutorAttributeTests
    {
        [Fact]
        public void Ctor_Throw_ArgumentNullException_Test()
        {
            Assert.Equal("type", Assert.Throws<ArgumentNullException>(() => new BindingExecutorAttribute(null)).ParamName);
        }

        [Fact]
        public void CtorTest()
        {
            Assert.Equal(typeof(string), new BindingExecutorAttribute(typeof(string)).Type);
        }
    }
}
