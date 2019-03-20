namespace OrderingExample.Config
{
    public interface IAppSettingsIndex
    {
        string this[string key] { get;  }
    }
}
