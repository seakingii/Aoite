using System.Data;
using Xunit;

namespace Aoite.Data
{
    public class AoiteTableTests
    {
        [Fact()]
        public void Test()
        {
            PageTable t = new PageTable();
            t.Total = 10;
            Assert.Equal(10, t.Total);
        }
    }
}
