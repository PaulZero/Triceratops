using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Http.Api.Interfaces
{
    public interface IApiResponse
    {
        bool Success { get; set; }

        string Error { get; set; }
    }
}
