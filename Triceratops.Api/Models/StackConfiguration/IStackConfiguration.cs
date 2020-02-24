using System.Threading.Tasks;

namespace Triceratops.Api.Models.StackConfiguration
{
    interface IStackConfiguration
    {
        public Task DownloadImagesAsync();

        public Task BuildAsync();

        public Task StartAsync();

        public Task StopAsync();

        public Task DestroyAsync();
    }
}
