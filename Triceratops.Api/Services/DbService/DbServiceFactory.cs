using Microsoft.Extensions.Configuration;
using System;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DbService.Mongo;

namespace Triceratops.Api.Services.DbService
{
    public static class DbServiceFactory
    {
        public static IDbService CreateFromEnvironmentVariables()
        {
            var mongoUsername = Environment.GetEnvironmentVariable("MONGO_USERNAME");
            var mongoPassword = Environment.GetEnvironmentVariable("MONGO_PASSWORD");

            if (string.IsNullOrWhiteSpace(mongoUsername) || string.IsNullOrWhiteSpace(mongoPassword))
            {
                throw new Exception("Cannot start DB connection without MONGO_USERNAME and MONGO_PASSWORD environment variables!");
            }

            return new MongoDbService(mongoUsername, mongoPassword);
        }
    }
}
