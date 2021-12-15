using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Defence.In.Depth;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host
            // Demo 1 - The default configuration will on Windows, where IIS is availible,
            // run Kestrel in-process hosted by the IIS with the ASP.NET Core module as a reverse proxy. 
            // On e g Linux it will be hosted out-of process in Kestrel, and you need a reverse proxy like NGINX in front
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                // Demo 8 - Handle secretes using App Configuration and Key Vault
                // Note that MSI needs to be set up and secretes needs to refrence key vault
                // https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references
                .ConfigureAppConfiguration((hostingContext, config) =>
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
                })
                .UseStartup<Startup>()
            );
}