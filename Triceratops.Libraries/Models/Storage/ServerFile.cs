using System;

namespace Triceratops.Libraries.Models.Storage
{
    public class ServerFile
    {
        public string Name { get; set; }

        public string RelativePath { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
