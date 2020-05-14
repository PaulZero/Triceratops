using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.Validation;

namespace Triceratops.Libraries.Models.ServerConfiguration.Minecraft
{
    public class MinecraftConfiguration : AbstractServerConfiguration
    {
        public const string ConfigGroupName = "Minecraft";
        public const string RconGroupName = "Remote Administration";

        public const ushort DefaultGamePort = 25565;
        public const ushort DefaultRconPort = 25575;
        public const uint DefaultMaxPlayers = 8;

        [Required]
        [Display(Name = "Max Players", Description = "The maximum number of players that can connect to the game at a single time.")]
        [Range(1, 16)]
        [ConfigField(ConfigGroupName)]
        public uint MaxPlayers { get; set; }

        [Required]
        [Display(Name = "Remote Admin Port", Description = "The port used for remote administration of the server.")]
        [Port]
        [ConfigField(RconGroupName)]
        public ushort RconHostPort { get; set; }

        [JsonIgnore]
        public ushort RconContainerPort => DefaultRconPort;

        public override ushort ContainerPort => DefaultGamePort;

        public override ServerType ServerType => ServerType.Minecraft;

        public MinecraftConfiguration()
        {
            HostPort = DefaultGamePort;
            RconHostPort = DefaultRconPort;
            MaxPlayers = DefaultMaxPlayers;
        }

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

            if (!Validation.ValidatePortsInRange(
                HostPort,
                ContainerPort,
                RconHostPort,
                RconContainerPort
            ))
            {
                return false;
            }

            if (!Validation.ValidatePortsDistinct(HostPort, RconHostPort))
            {
                return false;
            }

            if (!Validation.ValidatePortsDistinct(ContainerPort, RconContainerPort))
            {
                return false;
            }

            return true;
        }
    }
}
