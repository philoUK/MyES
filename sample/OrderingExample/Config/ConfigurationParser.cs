namespace OrderingExample.Config
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using StructureMap.TypeRules;

    public static class ConfigurationParser
    {
        public static T Read<T>(IDictionary<string, string> settings, string area = null)
            where T : new()
        {
            return Read(settings, new T(), area);
        }

        public static T Read<T>(IDictionary<string, string> settings, T target, string area = null)
        {
            var index = new AppSettingsDictionaryWrapper(settings);
            return Read(index, target, area);
        }

        public static T Read<T>(IAppSettingsIndex settings, string area = null)
            where T : new()
        {
            return Read(settings, new T(), area);
        }

        public static T Read<T>(IAppSettingsIndex settings, T target, string area = null)
        {
            foreach (var prop in typeof(T).GetSettableProperties())
            {
                var key = string.IsNullOrEmpty(area) ? prop.Name : $"{area}:{prop.Name}";
                var settingString = settings[key];
                if (settingString != null)
                {
                    var setting = ConvertToType(settingString, prop.PropertyType);
                    prop.SetValue(target, setting);
                }
            }

            return target;
        }

        private static object ConvertToType(string settingString, Type type)
        {
            if (type.IsValueType && type.IsNullable())
            {
                type = type.GetInnerTypeFromNullable();
            }

            return Convert.ChangeType(settingString, type, CultureInfo.InvariantCulture);
        }
    }
}
