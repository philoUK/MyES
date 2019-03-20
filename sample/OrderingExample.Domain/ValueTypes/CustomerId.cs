namespace OrderingExample.Domain.ValueTypes
{
    using System;

    public class CustomerId
    {
        public CustomerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("CustomerId cannot be blank");
            }

            this.Value = value;
        }

        public string Value { get; }

        public static CustomerId Parse(string value)
        {
            try
            {
                return new CustomerId(value);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
