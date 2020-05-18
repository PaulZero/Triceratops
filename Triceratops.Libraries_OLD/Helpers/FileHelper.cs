using System;
using System.IO;
using System.Linq;

namespace Triceratops.Libraries.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] _editableExtensions = new[]
        {
            ".config",
            ".json",
            ".txt"
        };

        private static readonly string[] _readableExtensions = new[]
        {
            ".log"
        };


        public static bool CanEdit(string fileName)
        {
            try
            {
                var extension = Path.GetExtension(fileName);

                return _editableExtensions.Contains(extension);
            }
            catch
            {
                return false;
            }
        }

        public static bool CanRead(string fileName)
        {
            if (CanEdit(fileName))
            {
                return true;
            }

            try
            {
                var extension = Path.GetExtension(fileName);

                return _readableExtensions.Contains(extension);
            }
            catch
            {
                return false;
            }
        }
    }
}
