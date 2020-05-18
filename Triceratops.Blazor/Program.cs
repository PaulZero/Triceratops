using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Triceratops.Blazor.Libraries.Http;
using Triceratops.Libraries.Http.Api;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Clients.Storage;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;

namespace Triceratops.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient<ITriceratopsApiClient>(s => 
                new TriceratopsApiClient(
                    new BlazorHttpClient(
                        s.GetRequiredService<ILogger<ITriceratopsApiClient>>(),
                        s.GetRequiredService<HttpClient>()
            )));
            builder.Services.AddTransient<ITriceratopsStorageClient>(s => 
                new TriceratopsStorageClient(
                    new BlazorHttpClient(
                        s.GetRequiredService<ILogger<ITriceratopsStorageClient>>(),
                        s.GetRequiredService<HttpClient>()
            )));

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
