
using KindredApi.Models;
using KindredApi.Repositories;
using KindredApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WageringServiceSettings>(builder.Configuration.GetSection(nameof(WageringServiceSettings)));

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<ICustomerClient, CustomerClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<WageringServiceSettings>>().Value;
    client.BaseAddress = new Uri($"https://{settings.Host}");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<ICustomerClient, CustomerClient>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IEventProducer, EventProducer>();
builder.Services.AddSingleton<IEventStore, EventStore>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddHostedService<WagerService>();

builder.Host.UseWolverine();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/customer/{customerId}/stats", async (ICustomerService service, int customerId) =>
    {
       return await service.GetCustomerStat(customerId);
    })
    .WithName("GetCustomerStats");

app.Run();