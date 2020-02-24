using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration
{
    public class MinecraftBedrockStackConfiguration : AbstractStackConfiguration
    {
        public MinecraftBedrockStackConfiguration(IDockerService dockerService, string containerPrefix)
            : base(dockerService, containerPrefix)
        {
            _imageNames.Add("itzg/minecraft-bedrock-server");
        }

        public override async Task Build()
        {
            await DownloadImages();
            _containerIds.Add(await _dockerService.CreateContainer(
                "itzg/minecraft-bedrock-server",
                WithPrefix("minecraft"),
                new List<string>() { "AUTH=true" }
           ));
        }
    }
}
