namespace Core.Utils
{
    using System;

    public static class TypeExtensions
    {
        public static string ToLoadableString(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}
