using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public abstract class AbstractEndpointResponse
    {
        public string Error { get; set; }

        public bool Success { get; set; }
    }
}
