using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration.Minecraft
{
    public class MinecraftStackConfiguration : AbstractStackConfiguration
    {
        private const string ContainerName = "minecraft";

        private const string ImageName = "itzg/minecraft-server";

        public MinecraftStackConfiguration(IDockerService dockerService, string containerPrefix)
            : base(dockerService, containerPrefix)
        {
            AddImage(ImageName);
        }

        public override async Task BuildAsync()
        {
            await DownloadImagesAsync();

            await CreateContainerBuilder(ImageName, ContainerName)
                .UsePrefix()
                .CreateContainerAsync();
        }
    }
}
