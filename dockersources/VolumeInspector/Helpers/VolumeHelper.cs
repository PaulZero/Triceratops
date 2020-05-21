using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Triceratops.VolumeInspector.Helpers
{
    internal static class VolumeHelper
    {
        public static string GetRelativePathFromHash(string hash)
        {
            try
            {
                var bytes = Convert.FromBase64String(hash);
                var relativePath = Encoding.UTF8.GetString(bytes);

                var parts = relativePath.Split('/').Where(StringNotEmpty);

                if (parts.Any(StringIsDirectoryNavigator))
                {
                    return null;
                }

                return relativePath;
            }
            catch
            {
                return null;
            }            
        }

        public static string GetFullPathFromRelativePath(string relativePath)
        {
            if (relativePath.StartsWith('/'))
            {
                return $"/volumes{relativePath}";
            }

            return $"/volumes/{relativePath}";
        }

        public static string GetFullPathFromHash(string fileHash, bool mustExist = false)
        {
            var relativePath = GetRelativePathFromHash(fileHash);

            if (relativePath == null)
            {
                return null;
            }

            var fullPath = GetFullPathFromRelativePath(relativePath);

            if (mustExist && !File.Exists(fullPath))
            {
                return null;
            }

            return fullPath;
        }

        private static bool StringNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static bool StringIsDirectoryNavigator(string value)
        {
            return value == ".." || value == ".";
        }
    }
}
