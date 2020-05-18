using System;
using System.Collections.Generic;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Libraries.Models.ServerConfiguration
{
    public class ServerConfigurationFactory
    {
        public AbstractServerConfiguration CreateFromServerType(ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.Minecraft:
                    return new MinecraftConfiguration();

                case ServerType.Terraria:
                    return new TerrariaConfiguration();

                default:
                    throw new Exception($"Server type {serverType} is not currently supported for creation.");
            }
        }

        public AbstractServerConfiguration CreateFromDictionary(IDictionary<string, string> values)
        {
            if (values.ContainsKey("ServerType") && Enum.TryParse<ServerType>(values["ServerType"]?.ToString(), out var serverType))
            {
                switch (serverType)
                {
                    case ServerType.Minecraft:
                        return CreateConfig<MinecraftConfiguration>(values);

                    case ServerType.Terraria:
                        return CreateConfig<TerrariaConfiguration>(values);

                    default:
                        throw new Exception($"Server type {serverType} is not currently supported for creation.");
                }
            }

            throw new Exception("Invalid values given - Cannot create configuration.");
        }

        private TConfig CreateConfig<TConfig>(IDictionary<string, string> values)
            where TConfig : AbstractServerConfiguration, new()
        {
            var config = new TConfig();
            var configType = typeof(TConfig);

            foreach (var kvp in values)
            {
                var property = configType.GetProperty(kvp.Key);

                if (property?.CanWrite ?? false)
                {
                    property.SetValue(config, Convert.ChangeType(kvp.Value, property.PropertyType));
                }
            }

            return config;
        }
    }
}
