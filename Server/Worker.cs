using System.Text.Json;
using Common.Models;
using Confluent.Kafka;

namespace Server;

public class Worker(IProducer<string, string> producer, BaseMessage[] messages) : BackgroundService
{
    const string Topic = Common.Constants.Topic;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        foreach (var message in messages)
        {
            Thread.Sleep(75);
            var json = JsonSerializer.Serialize(message);
            var result = await producer.ProduceAsync(Topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            }, ct);
            
            if (result.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {message.Type} message to topic - {Topic} due to the following reason: {result.Message}.");
            }
        }
        
    }

    public override void Dispose()
    {
        producer.Dispose();
        base.Dispose();
    }
}