using System.Text.Json;
using Common.Models;
using Confluent.Kafka;

namespace KindredApi.Services;

public class Worker(IConsumer<string, string> consumer, IEventProducer eventProducer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        consumer.Subscribe(Common.Constants.Topic);
        
        while (!ct.IsCancellationRequested)
        {
            var result = consumer.Consume(ct);
            if (result?.Message == null) continue;
            
            var @event = JsonSerializer.Deserialize<BaseMessage>(result.Message.Value);
            if (@event!.Type == MessageType.EndofFeed)
            {
                //disconnect from the server
                consumer.Close();
            }
            else
            {
                //publish the event to the event bus (Currently implemented using wolverine)
                await eventProducer.PublishAsync(@event);
            }
            consumer.Commit(result);
            
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}