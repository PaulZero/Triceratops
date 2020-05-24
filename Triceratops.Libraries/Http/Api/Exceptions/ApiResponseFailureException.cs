using System;

namespace Triceratops.Libraries.Http.Api.Exceptions
{
    public class ApiResponseFailureException : Exception
    {
        public ApiResponseFailureException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
