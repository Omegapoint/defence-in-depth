var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
