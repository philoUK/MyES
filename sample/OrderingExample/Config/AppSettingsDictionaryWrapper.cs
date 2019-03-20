namespace OrderingExample.Config
{
    using System.Collections.Generic;

    public class AppSettingsDictionaryWrapper : IAppSettingsIndex
    {
        private readonly IDictionary<string, string> dictionary;

        public AppSettingsDictionaryWrapper(IDictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        public string this[string key] => this.dictionary.ContainsKey(key) ? this.dictionary[key] : null;
    }
}
