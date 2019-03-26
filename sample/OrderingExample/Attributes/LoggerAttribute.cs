namespace OrderingExample.Attributes
{
    using System;
    using Microsoft.Azure.WebJobs.Description;

    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class LoggerAttribute : Attribute
    {
        public string Function { get; set; }
    }
}
