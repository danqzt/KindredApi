using KindredApi.Models;
using KindredApi.Repositories;
using KindredApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WageringServiceSettings>(builder.Configuration.GetSection(nameof(WageringServiceSettings)));

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<ICustomerClient, CustomerClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<WageringServiceSettings>>().Value;
    client.BaseAddress = new Uri($"http://{settings.Host}/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IEventProducer, EventProducer>();
builder.Services.AddSingleton<IEventStore, EventStore>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddHostedService<WagerService>();

builder.Host.UseWolverine(opts =>
{
    opts.UseFluentValidation();
});

var app = builder.Build();

// Global exception handler middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = new { message = "An unexpected error occurred.", detail = ex.Message };
        await context.Response.WriteAsJsonAsync(error);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/customer/{customerId}/stats", 
        async (ICustomerService service, int customerId) =>
        {
            if (customerId <= 0)
                return Results.BadRequest(new { message = "customerId must be greater than 0."});
            
            //Can be improved to use Mediatr pattern 
            var resp = await service.GetCustomerStat(customerId);
            if (resp == null) return Results.NotFound(); 
            return Results.Ok(resp);
        })
    .WithName("GetCustomerStats");

app.Run();