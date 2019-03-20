namespace OrderingExample.Config
{
    using Microsoft.Extensions.Options;

    public class OptionsProvider<T> : IOptions<T>
        where T : class, new()
    {
        public OptionsProvider(string configSection)
        {
            var options = ConfigurationParser.Read<T>(new EnvironmentVariablesIndex(), configSection);
            this.Value = options;
        }

        public T Value { get; }
    }
}
