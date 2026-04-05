using Common.Extensions;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Server;


var builder = WebApplication.CreateBuilder(args);

var wageringSettings = builder.ConfigureSettings<WageringServiceSettings>();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddDbContext<CustomerDbContext>(opt => opt.UseInMemoryDatabase("CustomerDb"));
builder.Services.AddSingleton(Constants.Events);
if (wageringSettings.UseKafka)
{
    builder.AddKafkaProducer<string, string>("kafka");
    builder.Services.AddHostedService<Worker>();
}

var app = builder.Build();

// Seed the database with customers from Constants.CUST_IDS
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    if (!db.Customers.Any())
    {
        foreach (var id in Constants.CUST_IDS)
        {
            db.Customers.Add(new Customer { Id = id, CustomerName = $"{Faker.Name.First()} {Faker.Name.Last()}" });
        }
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable WebSockets
app.UseWebSockets(new()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});

app.MapGet("/customer", async (int customerId, CustomerDbContext db) =>
{
    var customer = await db.Customers.FindAsync(customerId);
    if (customer == null)
    {
        return Results.NotFound($"Customer with Id {customerId} not found.");
    }
    return Results.Ok(customer);
})
.WithName("GetCustomer");

// WebSocket endpoint
app.Map("/ws", async (HttpContext context, BaseMessage[] messages, ILogger<Program> logger) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        
        // 1. Create a timeout source for 10 minutes
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // 2. Link the timeout with the browser's disconnect signal
        // This ensures we stop for BOTH: 10 min expiry OR client closing the tab
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            context.RequestAborted, 
            timeoutCts.Token);
    
        var ct = linkedCts.Token;
        foreach (var message in messages)
        {
            Thread.Sleep(100);
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, ct);
        }
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done sending messages", ct);
        logger.LogInformation("=== WebSocket closed === ");
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
