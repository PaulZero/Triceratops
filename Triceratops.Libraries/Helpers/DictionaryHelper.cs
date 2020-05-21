using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Triceratops.Libraries.Helpers
{
    public static class DictionaryHelper
    {
        public static IDictionary<string, object> ObjectToDictionary(object data)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(data))
            {
                dictionary.Add(property.Name, property.GetValue(data));
            }

            return dictionary;
        }
    }
}
