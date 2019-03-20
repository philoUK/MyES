namespace OrderingExample.Domain.ValueTypes
{
    using System;

    public class OrderNumber
    {
        public OrderNumber(string value)
        {
            if ((value?.Length ?? 0) != 6)
            {
                throw new ArgumentException("OrderNumber must be 6 characters long");
            }

            this.Value = value;
        }

        public string Value { get; }

        public static OrderNumber Parse(string value)
        {
            try
            {
                return new OrderNumber(value);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
