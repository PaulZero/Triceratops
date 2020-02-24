using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Models.StackConfiguration
{
    interface IStackConfiguration
    {
        public Task DownloadImages();

        public Task Build();

        public Task Start();

        public Task Stop();

        public Task Destroy();
    }
}
