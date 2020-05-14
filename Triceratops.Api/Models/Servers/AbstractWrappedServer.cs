using System;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Api.Models.Servers
{
    public abstract class AbstractWrappedServer<TConfig>
        where TConfig : AbstractServerConfiguration
    {
        public ServerType ServerType => ServerConfiguration.ServerType;

        public Server ServerEntity { get; private set; }

        protected TConfig ServerConfiguration { get; }

        public AbstractWrappedServer(Server serverEntity)
        {
            if (serverEntity.ConfigurationType != typeof(TConfig))
            {
                throw new Exception("Invalid server type specified!");
            }

            ServerEntity = serverEntity;
            ServerConfiguration = serverEntity.DeserialiseConfiguration() as TConfig;
        }
    }
}