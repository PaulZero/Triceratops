using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration.MinecraftBedrock
{
    public class MinecraftBedrockStackConfiguration : AbstractStackConfiguration
    {
        private const string ContainerName = "minecraft-bedrock";

        private const string ImageName = "itzg/minecraft-bedrock-server";

        public MinecraftBedrockStackConfiguration(IDockerService dockerService, string containerPrefix)
            : base(dockerService, containerPrefix)
        {
            AddImage(ImageName);
        }

        public override async Task BuildAsync()
        {
            await DownloadImagesAsync();

            await CreateContainerBuilder(ImageName, ContainerName)
                .UsePrefix()
                .WithEnvironmentVariables("AUTH=true")
                .CreateContainerAsync();
        }
    }
}
