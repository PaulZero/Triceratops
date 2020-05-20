using DnsClient.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Triceratops.Api.Models.ActionFilters;
using Triceratops.Api.Services.DbService;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Http.Clients.Storage;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;
using Triceratops.Libraries.RouteMapping;
using Triceratops.Libraries.RouteMapping.Interfaces;

namespace Triceratops.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(o =>
            {
                o.Filters.Add(new TimedRequestAttribute());
            });

            ConfigureDocker(services);

            services.AddSingleton(s => DbServiceFactory.CreateFromEnvironmentVariables(s.GetRequiredService<IConfiguration>()));
            services.AddSingleton(s => s.GetRequiredService<IDbService>().Servers);
            services.AddSingleton(s => s.GetRequiredService<IDbService>().Containers);

            services.AddSingleton<ITriceratopsStorageClient>(s => new TriceratopsStorageClient(new CoreHttpClient(s.GetRequiredService<ILogger<ITriceratopsStorageClient>>())));

            services.AddSingleton<IServerService>(s => new ServerService(
                s.GetRequiredService<IDbService>(),
                s.GetRequiredService<IDockerService>(),
                s.GetRequiredService<ITriceratopsStorageClient>()
            ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureDocker(IServiceCollection services)
        {
            var dockerDaemonUrl = Environment.GetEnvironmentVariable("DOCKER_DAEMON_URL");

            if (string.IsNullOrWhiteSpace(dockerDaemonUrl))
            {
                throw new Exception($"The environment variable DOCKER_DAEMON_URL must be set for Triceratops to run!");
            }

            services.AddSingleton<IDockerService>(s => new DockerService(dockerDaemonUrl, s.GetRequiredService<ILogger<IDockerService>>()));
        }
    }
}
