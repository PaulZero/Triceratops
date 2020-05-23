namespace Triceratops.Libraries.RouteMapping.Enums
{
    public enum HttpMethod
    {
        Get,
        Post
    }

    public static class HttpMethodExtensions
    {
        public static string GetString(this HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.Get:
                    return "GET";

                case HttpMethod.Post:
                    return "POST";

                default:
                    return null;
            }
        }
    }
}
