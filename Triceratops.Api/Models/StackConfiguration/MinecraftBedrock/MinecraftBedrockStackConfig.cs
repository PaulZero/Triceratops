using Triceratops.Api.Models.StackConfiguration.MinecraftBedrock.Enums;

namespace Triceratops.Api.Models.StackConfiguration.MinecraftBedrock
{
    public class MinecraftBedrockStackConfig
    {
        public MinecraftBedrockGameMode GameType { get; set; } = MinecraftBedrockGameMode.Survival;

        public MinecraftBedrockDifficulty Difficulty { get; set; } = MinecraftBedrockDifficulty.Normal;
    }
}
