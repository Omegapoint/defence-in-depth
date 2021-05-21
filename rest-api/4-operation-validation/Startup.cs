using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _4_operation_validation
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Move code from rest-sec-net from ProductId.  We can choose
            // code from lab 4 or lab 6.  The main point here is to emphasize 
            // early validation of operation.  I.e. only scope.

            // What we need to decide is if we want to introduce the domain model
            // now, or just code directly against the model from ClaimsTranformation
            // in the controller.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
