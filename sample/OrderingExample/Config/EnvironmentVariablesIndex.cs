namespace OrderingExample.Config
{
    using System;

    public class EnvironmentVariablesIndex : IAppSettingsIndex
    {
        public string this[string key] => Environment.GetEnvironmentVariable(key);
    }
}
