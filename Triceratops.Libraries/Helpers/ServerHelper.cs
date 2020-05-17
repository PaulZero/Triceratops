using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triceratops.Libraries.Helpers
{
    public static class ServerHelper
    {
        public static string CreateServerNameSlug(string serverName)
        {
            var characters = serverName.ToLower().Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray();
            var slug = new string(characters);

            return slug.Replace(' ', '_');
        }
    }
}
