namespace Triceratops.Api.Models
{
    /// <summary>
    /// A horrible piece of built in ASP.NET. If you delete it, it won't build
    /// (it's used in an error route, <see cref="Startup"/>).
    /// </summary>

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
