using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Triceratops.VolumeInspector.Requests;

namespace Triceratops.VolumeInspector
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet<VolumeTreeRequest>("/");
                endpoints.MapGet<DownloadZipRequest>("/download-zip");

                endpoints.MapGet<DownloadFileRequest>("/download/{fileHash}");                
                endpoints.MapPost<UploadFileRequest>("/upload/{fileHash}");
                endpoints.MapPost<DeleteFileRequest>("/delete/{fileHash}");

                endpoints.MapGet<VerifyServerRunningRequest>("/verify");
                endpoints.MapFallback<MissingEndpointRequest>();
            });
        }
    }

    internal static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGet<TRequest>(this IEndpointRouteBuilder endpoints, string pattern)
            where TRequest : AbstractRequest, new()
        {
            return endpoints.MapGet(pattern, new RequestHandler().HandleRequest<TRequest>);
        }

        public static IEndpointConventionBuilder MapPost<TRequest>(this IEndpointRouteBuilder endpoints, string pattern)
            where TRequest : AbstractRequest, new()
        {
            return endpoints.MapPost(pattern, new RequestHandler().HandleRequest<TRequest>);
        }

        public static IEndpointConventionBuilder MapFallback<TRequest>(this IEndpointRouteBuilder endpoints)
            where TRequest : AbstractRequest, new()
        {
            return endpoints.MapFallback(new RequestHandler().HandleRequest<TRequest>);
        }
    }
}
