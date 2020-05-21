using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal abstract class AbstractRequest
    {
        private HttpContext _context;

        public async Task<IResponse> ExecuteAsync(HttpContext context)
        {
            _context = context;

            try
            {
                CheckVolumesExist();

                return await CreateResponseAsync();
            }
            catch (Exception exception)
            {
                return new ServerErrorResponse(exception);
            }
        }

        protected abstract Task<IResponse> CreateResponseAsync();

        protected string GetRouteValue(string name)
        {            
            return _context.GetRouteValue(name)?.ToString();
        }

        protected Stream GetRequestStream()
        {
            return _context.Request.Body;
        }

        protected Task<IResponse> TaskResponse(IResponse response)
        {
            return Task.FromResult(response);
        }        

        private void CheckVolumesExist()
        {
            if (!Directory.Exists("/volumes"))
            {
                throw new Exception("/volumes mount does not exist!");
            }
        }
    }
}
