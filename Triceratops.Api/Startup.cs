using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Triceratops.Api.Services.DbService;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Services.ServerService;

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
            services.AddControllersWithViews();

            services.AddSingleton<IDockerService>(s => new DockerService());            

            services.AddSingleton(s => DbServiceFactory.CreateFromEnvironmentVariables(s.GetRequiredService<IConfiguration>()));
            services.AddSingleton(s => s.GetRequiredService<IDbService>().Servers);
            services.AddSingleton(s => s.GetRequiredService<IDbService>().Containers);

            services.AddSingleton<IServerService>(s => new ServerService(
                s.GetRequiredService<IDbService>(),
                s.GetRequiredService<IDockerService>()
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
    }
}
