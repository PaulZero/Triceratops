using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Models.Api.Response
{
    public class ApiModelResponse<T>
        where T : class, new()
    {
        public T Model { get; set; }

        public ApiModelResponse(T model)
        {
            Model = model;
        }
    }
}
