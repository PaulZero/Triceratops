using System;
using System.Text;

namespace Triceratops.Libraries.Helpers
{
    public static class HashHelper
    {
        public static string CreateHash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(bytes);
        }

        public static string CreateString(string hash)
        {
            var bytes = Convert.FromBase64String(hash);

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
