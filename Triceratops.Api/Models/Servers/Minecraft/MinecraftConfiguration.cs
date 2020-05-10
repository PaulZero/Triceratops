namespace Triceratops.Api.Models.Servers.Minecraft
{
    public class MinecraftConfiguration : AbstractServerConfiguration
    {
        public string ServerName { get; set; }

        public uint MaxPlayers { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ServerName))
            {
                return false;
            }

            if (MaxPlayers < 1 || MaxPlayers > 16)
            {
                return false;
            }

            return true;
        }
    }
}
