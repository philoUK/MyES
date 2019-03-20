namespace Core
{
    using System;

    public interface IAggregateEvent
    {
        int Version { get; set; }

        string Id { get; set; }

        DateTime DateOfEvent { get; set; }
    }
}