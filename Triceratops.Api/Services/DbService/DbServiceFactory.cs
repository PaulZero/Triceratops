using Microsoft.Extensions.Configuration;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DbService.Mongo;

namespace Triceratops.Api.Services.DbService
{
    public static class DbServiceFactory
    {
        public static IDbService CreateFromEnvironmentVariables(IConfiguration config)
        {
            return new MongoDbService();
        }
    }
}
