using System;

namespace Triceratops.Api.Models.Servers
{
    public abstract class AbstractWrappedServer<TConfig>
        where TConfig : AbstractServerConfiguration
    {
        public abstract ServerType ServerType { get; }

        protected Server ServerEntity { get; }

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