using Projects;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Server>("server");
builder.AddProject<KindredApi>("stats-api");
builder.Build().Run();