﻿using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models;

namespace Triceratops.Libraries.Helpers
{
    public static class NameHelper
    {
        private static readonly string _dockerEntityPrefix = "Triceratops";

        public static string CreateContainerName(Server server, ServerType serverType, string customName = null)
        {
            if (string.IsNullOrWhiteSpace(customName))
            {
                return $"{_dockerEntityPrefix}.{serverType}.{server.Slug}";
            }

            return $"{_dockerEntityPrefix}.{serverType}.{server.Slug}.{customName}";
        }

        public static string CreateVolumeName(Server server, ServerType serverType, string customName)
        {
            return $"{_dockerEntityPrefix}.{serverType}.{server.Slug}.{customName}";
        }

        public static string SanitiseHostname(string hostName)
        {
            var characters = hostName
                .Where(c => char.IsLetter(c) || c == ' ')
                .Select(c => c == ' ' ? '.' : c)
                .ToArray();

            return new string(characters);
        }
    }
}
