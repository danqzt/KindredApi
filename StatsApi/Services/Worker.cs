using System.Text.Json;
using Common.Models;
using Confluent.Kafka;

namespace KindredApi.Services;

public class Worker(IConsumer<string, string> consumer, IEventProducer eventProducer, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        consumer.Subscribe(Common.Constants.Topic);

        while (!ct.IsCancellationRequested)
        {
            ConsumeResult<string, string>? result = null;
            try
            {
                result = consumer.Consume(ct);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Consume operation cancelled by the user.");
                break;
            }
            catch (ObjectDisposedException)
            {
                logger.LogWarning("Consumer is disposed.");
                break;
            }

            if (result?.Message == null) continue;

            var @event = JsonSerializer.Deserialize<BaseMessage>(result.Message.Value);
            if (@event!.Type == MessageType.EndofFeed)
            {
                //disconnect from the server
                logger.LogInformation("=== END OF FEED ===");
                consumer.Close();
                break;
            }

            //publish the event to the event bus (Currently implemented using wolverine)
            await eventProducer.PublishAsync(@event);
            consumer.Commit(result);
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}