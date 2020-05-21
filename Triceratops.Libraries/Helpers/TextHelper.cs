using System.Collections.Generic;
using System.Linq;

namespace Triceratops.Libraries.Helpers
{
    public static class TextHelper
    {
        public static string Pluralise<T>(string singular, IEnumerable<T> collection)
            => Pluralise(singular, collection.Count());
        
        public static string Pluralise(string singular, int quantity)
        {
            if (quantity == 1)
            {
                return singular;
            }

            if (singular.EndsWith('s'))
            {
                return $"{singular}es";
            }

            return $"{singular}s";
        }
    }
}
