using Microsoft.Extensions.Configuration;
using System;
using Triceratops.Api.Services.DbService.Memory;

namespace Triceratops.Api.Services.DbService
{
    public static class DbServiceFactory
    {
        /// <summary>
        /// Constructs new MySQL DB service from the specified environment variables:
        /// 
        /// DB_HOST
        /// DB_USER
        /// DB_PASS
        /// DB_SCHEMA
        /// 
        /// If DB_USE_MEMORY is specified, then the above are ignored and the in-memory test DB is used.
        /// </summary>
        /// <returns></returns>
        public static IDbService CreateFromEnvironmentVariables(IConfiguration config)
        {
            var useMemoryDb = config.GetValue("DB_USE_MEMORY", false);

            if (useMemoryDb)
            {
                return new MemoryDb();
            }

            //var host = config.GetValue<string>("DB_HOST", null);
            //var user = config.GetValue<string>("DB_USER", null);
            //var pass = config.GetValue<string>("DB_PASS", null);
            //var schema = config.GetValue<string>("DB_SCHEMA", null);

            throw new Exception("Cannot connect to a non-memory DB right now, go and set the DB_USE_MEMORY environment variable to true");
        }
    }
}
