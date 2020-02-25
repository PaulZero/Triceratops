using System;
using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Models.StackConfiguration.MinecraftBedrock.Enums;

namespace Triceratops.Api.Models.StackConfiguration.MinecraftBedrock
{
    public class MinecraftBedrockStack : AbstractStack
    {
        private const string ContainerName = "minecraft-bedrock";

        private const string ImageName = "itzg/minecraft-bedrock-server";

        private readonly MinecraftBedrockStackConfig _config = new MinecraftBedrockStackConfig();

        public MinecraftBedrockStack(IDockerService dockerService, ContainerStack stack)
            : base(dockerService, stack)
        {
            _config = stack.GetConfig<MinecraftBedrockStackConfig>();

            if (!stack.IsForStack(this))
            {
                throw new Exception("Stack mismatch!");
            }

            AddImage(ImageName);
        }

        public override async Task BuildAsync()
        {
            await DownloadImagesAsync();

            await CreateContainerBuilder(ImageName, ContainerName)
                .UsePrefix()
                .WithEnvironmentVariables(
                    "EULA=TRUE",
                    _config.GameType.GetEnvironmentVariable(),
                    _config.Difficulty.GetEnvironmentVariable())
                .CreateContainerAsync();
        }
    }
}
