namespace Core.Tests
{

    public class AppendTestData : AggregateEvent
    {
        public AppendTestData()
        {
        }

        public AppendTestData(string data)
        {
            this.Data = data;
        }

        public string Data { get; set; }
    }
}
