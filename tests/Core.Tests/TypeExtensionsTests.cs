namespace Core.Tests
{
    using System;
    using Utils;
    using Xunit;

    public class TypeExtensionsTests
    {
        [Fact]
        public void CheckName()
        {
            Assert.Equal("Core.Tests.TypeExtensionsTests, Core.Tests", this.GetType().ToLoadableString());
        }

        [Fact]
        public void CheckTypeIsLoadable()
        {
            var myTypeSpecification = this.GetType().ToLoadableString();
            var myType = Type.GetType(myTypeSpecification);
            Assert.True(myType == this.GetType());
        }
    }
}
