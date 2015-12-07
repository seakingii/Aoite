using Xunit;
namespace System
{
    public class GridDataTests
    {
        [Fact()]
        public void Test()
        {
            PageData<int> grid = new PageData<int>()
            {
                Total = 5,
                Rows = new int[] { 1, 2, 3, 4, 5 }
            };
            Assert.Equal(5, grid.Total);
            Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, grid.Rows);
        }
    }
}
