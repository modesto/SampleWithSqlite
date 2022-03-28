using System;

namespace Starter.Tests.SampleModels
{
    public class OutboxRecord
    {
        public int Id { get; set; }
        public string EventId { get; set; }
        public string Payload { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}