using System;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Libraries.Enums;

namespace Triceratops.Api.Models.Servers
{
    public class ServerHelper
    {
        public ServerType GetServerTypeForConfigurationType(Type configurationType)
        {
            if (!typeof(AbstractServerConfiguration).IsAssignableFrom(configurationType))
            {
                throw new Exception($"Invalid type specified - must be subclass of {nameof(AbstractServerConfiguration)}");
            }

            if (configurationType == typeof(MinecraftConfiguration))
            {
                return ServerType.Minecraft;
            }

            return ServerType.Unknown;
        }
    }
}
