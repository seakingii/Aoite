using Xunit;

namespace System
{
    public class IgnoreAttributeTests
    {
        [Fact()]
        public void Type()
        {
            Assert.Equal(typeof(IgnoreAttribute), IgnoreAttribute.Type);
        }
    }
}
