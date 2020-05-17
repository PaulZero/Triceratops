using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.ApiService.Interfaces;

namespace Triceratops.Dashboard.Services.ApiService
{
    public class ApiService : IApiService
    {
        private const string BaseUrl = "http://triceratops.api";

        public IServerApi Servers { get; }

        public ApiService(ILoggerFactory loggerFactory)
        {
            Servers = new ServerApi(BaseUrl, loggerFactory.CreateLogger<IServerApi>());
        }
    }
}
