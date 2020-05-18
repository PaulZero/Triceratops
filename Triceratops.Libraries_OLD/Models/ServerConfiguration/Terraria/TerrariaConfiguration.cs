using Triceratops.Libraries.Enums;

namespace Triceratops.Libraries.Models.ServerConfiguration.Terraria
{
    public class TerrariaConfiguration : AbstractServerConfiguration
    {
        public const ushort DefaultServerPort = 7777;

        public override ushort ContainerPort => DefaultServerPort;

        public override ServerType ServerType => ServerType.Terraria;

        public TerrariaConfiguration()
        {
            HostPort = DefaultServerPort;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
