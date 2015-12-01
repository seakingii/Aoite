using Xunit;

namespace Aoite.Data
{
    public class AoiteTableTests
    {
        [Fact()]
        public void Test()
        {
            AoiteTable t = new AoiteTable();
            t.TotalRowCount = 10;
            Assert.Equal(10, t.TotalRowCount);
        }
    }
}
