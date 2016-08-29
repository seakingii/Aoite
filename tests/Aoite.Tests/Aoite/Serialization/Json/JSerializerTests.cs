using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.Serialization.Json
{
    public class JSerializerTests
    {
        Xunit.Abstractions.ITestOutputHelper m_ouput;
        public JSerializerTests(Xunit.Abstractions.ITestOutputHelper ouput)
        {
            m_ouput = ouput;
        }
        [Fact]
        public void ResultTest()
        {
            m_ouput.WriteLine(Serializer.Json.FastWrite(new Result("aa", 12)));
        }
    }
}
