using System;
using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Libraries.Helpers
{
    public static class ServerTypeHelper
    {
        private static ServerType[] DisallowedServerTypes = new[]
        {
            ServerType.Unknown
        };

        public static ServerType[] AllowedServerTypes => 
            Enum.GetValues(typeof(ServerType))
            .Cast<ServerType>()
            .Where(s => !DisallowedServerTypes.Contains(s))
            .ToArray();

        public static ServerType GetServerTypeForConfiguration(AbstractServerConfiguration configuration)
        {
            return GetServerTypeForConfiguration(configuration.GetType());
        }

        public static ServerType GetServerTypeForConfiguration(Type type)
        {
            if (type == typeof(MinecraftConfiguration))
            {
                return ServerType.Minecraft;
            }

            if (type == typeof(TerrariaConfiguration))
            {
                return ServerType.Terraria;
            }

            return ServerType.Unknown;
        }
    }
}
