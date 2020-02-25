namespace Triceratops.Api.Models.StackConfiguration.MinecraftBedrock.Enums
{
    public enum MinecraftBedrockGameMode
    {
        Creative,
        Survival
    }

    public static class MinecraftBedrockGameModeExtensions
    {
        public static string GetEnvironmentVariable(this MinecraftBedrockGameMode gameType)
        {
            return $"GAMEMODE={GetGameModeLabel(gameType)}";
        }

        private static string GetGameModeLabel(MinecraftBedrockGameMode gameType)
        {
            return gameType switch
            {
                MinecraftBedrockGameMode.Creative => "creative",
                _ => "survival",
            };
        }
    }
}
