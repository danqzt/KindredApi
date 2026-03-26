using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using KindredApi.Models;
using KindredApi.Models.External;
using Microsoft.Extensions.Options;
using Wolverine;
using Wolverine.Runtime;

namespace KindredApi.Services;

public class WagerService(IOptions<WageringServiceSettings> config, IEventProducer eventProducer) : BackgroundService
{
    private WageringServiceSettings Settings => config.Value;
    private readonly ClientWebSocket _client = new ClientWebSocket();

    protected override async Task ExecuteAsync(CancellationToken cts)
    {
        await _client.ConnectAsync(
            new Uri(
                $"ws://{Settings.Host}/ws?candidateId={Settings.CandidateId}"),
            cts);
        var buffer = new byte[1024];

        while (!cts.IsCancellationRequested && _client.State == WebSocketState.Open)
        {
            var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), cts);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                // stop consuming
                break;
            }

            var baseMessageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var baseMessage = JsonSerializer.Deserialize<BaseMessage>(baseMessageJson);
            await eventProducer.PublishAsync(baseMessage!);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client.State == WebSocketState.Open)
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stopping", cancellationToken);
        }
    }

    public override void Dispose()
    {
        _client.Dispose();
        base.Dispose();
    }
}