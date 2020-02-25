namespace Triceratops.Api.Models.StackConfiguration.MinecraftBedrock.Enums
{
    public enum MinecraftBedrockDifficulty
    {
        Easy,
        Normal
    }

    public static class MinecraftBedrockDifficultyExtensions
    {
        public static string GetEnvironmentVariable(this MinecraftBedrockDifficulty difficulty)
        {
            return $"DIFFICULTY={GetDifficultyLabel(difficulty)}";
        }

        private static string GetDifficultyLabel(MinecraftBedrockDifficulty difficulty)
        {
            return difficulty switch
            {
                MinecraftBedrockDifficulty.Easy => "easy",
                _ => "normal",
            };
        }
    }
}
