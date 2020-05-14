using System;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;

namespace Triceratops.Api.Models.Servers
{
    public class ServerHelper
    {
        public ServerType GetServerTypeForConfigurationType(Type configurationType)
        {
            if (!typeof(AbstractServerConfiguration).IsAssignableFrom(configurationType))
            {
                throw new Exception($"Invalid type specified - {configurationType.Name} must be subclass of {nameof(AbstractServerConfiguration)}");
            }

            if (configurationType == typeof(MinecraftConfiguration))
            {
                return ServerType.Minecraft;
            }

            return ServerType.Unknown;
        }
    }
}
