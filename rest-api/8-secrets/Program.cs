using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => options
    .Connect(
        new Uri(builder.Configuration["AzureAppConfiguration:Url"] ?? string.Empty), 
        new DefaultAzureCredential())
    .ConfigureKeyVault(c => c.SetCredential(new DefaultAzureCredential())));

var app = builder.Build();

app.MapGet("/", () => "Hello World!")
            .RequireAuthorization();

app.Run();
