using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureAppConfiguration((_, config) =>
{
    var settings = config.Build();
    var credentials = new DefaultAzureCredential();

    config.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(settings["AzureAppConfiguration:Url"]), credentials)
            .ConfigureKeyVault(kv =>
            {
                kv.SetCredential(credentials);
            });
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
