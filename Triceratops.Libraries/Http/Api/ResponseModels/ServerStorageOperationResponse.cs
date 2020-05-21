using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerStorageOperationResponse
    {
        public static ServerStorageOperationResponse SuccessfulResponse => new ServerStorageOperationResponse
        {
            Success = true
        };

        public static ServerStorageOperationResponse FailedResponse => new ServerStorageOperationResponse
        {
            Success = false
        };

        public bool Success { get; set; }

        
    }
}
