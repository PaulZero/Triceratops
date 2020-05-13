namespace Triceratops.Libraries.Models.Api.Response
{
    public abstract class StandardResponse
    {
        public bool Success { get; }

        public string Message { get; }

        public object Value { get; }

        public StandardResponse(bool success, string message = null, object value = null)
        {
            Success = success;
            Message = message;
            Value = value;
        }
    }
}
