using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Triceratops.Libraries.Helpers
{
    public static class JsonHelper
    {
        public static void UpdateSerialiserOptions(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.IgnoreReadOnlyProperties = true;
        }

        public static string Serialise(object value)
        {
            return JsonSerializer.Serialize(value, CreateOptions());
        }

        public static T Deserialise<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, CreateOptions());
        }

        public static object Deserialise(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, CreateOptions());
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();

            UpdateSerialiserOptions(options);

            return options;
        }
    }
}
