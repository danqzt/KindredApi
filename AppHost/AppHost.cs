using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddKafka("kafka")
    .WithKafkaUI();

builder.AddProject<Server>("server")
    .WithReference(kafka)
    .WaitFor(kafka);

builder.AddProject<KindredApi>("stats-api")
    .WithReference(kafka)
    .WaitFor(kafka);
builder.Build().Run();