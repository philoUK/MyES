namespace Core.Tests
{
    using System;
    using Core.Utils;
    using Newtonsoft.Json;
    using Xunit;

    public class SerializerTests
    {
        [Fact]
        public void CanSerialiseAndDeSerialiseAnEmbeddedClass()
        {
            var target = new EmbeddedTest.Command { TestSetting = "test123" };
            var json = JsonConvert.SerializeObject(target);
            var targetTypeString = target.GetType().ToLoadableString();
            var targetType = Type.GetType(targetTypeString);
            var loadedTarget = (EmbeddedTest.Command)JsonConvert.DeserializeObject(json, targetType);
            Assert.Equal(target.TestSetting, loadedTarget.TestSetting);
        }
    }
}
