using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace _9_complete_with_all_defence_layers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Demo 1 - The default configuration will on Windows, where IIS is availible,
        // run in-process hosted by the IIS ASP.NET Core module as a reverse proxy. 
        // On e g Linux it will be hosted out-of process in Kestrel, and you need a reverse proxy like NGINX in front
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
