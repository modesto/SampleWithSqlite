using System;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Starter.Tests.SampleModels;
using Xunit;

namespace Starter.Tests
{
    public class BasicTester
    {
        [Fact]
        public async Task works_with_sqlite()
        {
            await using var connection = new SqliteConnection("Data Source=sample.db");
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                @"CREATE TABLE IF NOT EXISTS Outbox (
                        Id INTEGER PRIMARY KEY,
                        EventId TEXT NOT NULL UNIQUE,
                        Payload TEXT NOT NULL,
                        PublishedAt TEXT NOT NULL
                    )");
            await connection.ExecuteAsync("DELETE FROM Outbox");

            var sampleEvent = new UserCreated()
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Name = "John",
                Surname = "Doe",
                Email = "any@email.com",
            };

            var outboxSample = new OutboxRecord()
            {
                EventId = sampleEvent.Id,
                Payload = JsonConvert.SerializeObject(sampleEvent,Formatting.Indented),
                PublishedAt = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                @"INSERT INTO Outbox (EventId, Payload, PublishedAt)
                    VALUES (@EventId, @Payload, @PublishedAt)", outboxSample);

            var insertedRow = await connection.QueryFirstAsync<OutboxRecord>(
                @"SELECT Id, EventId, Payload, PublishedAt FROM Outbox where Id=1");

            insertedRow.Id.Should().Be(1);
            insertedRow.EventId.Should().Be(sampleEvent.Id);
        }
    }
}