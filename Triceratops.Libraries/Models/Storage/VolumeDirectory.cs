using System.Collections.Generic;
using System.Linq;

namespace Triceratops.Libraries.Models.Storage
{
    public class VolumeDirectory
    {
        public string Name { get; set; }

        public string RelativePath { get; set; }

        public bool HasFiles => Files?.Any() ?? false;

        public bool HasDirectories => Directories?.Any() ?? false;

        public bool IsEmpty => !HasFiles && !HasDirectories;

        public IEnumerable<VolumeFile> Files { get; set; }

        public IEnumerable<VolumeDirectory> Directories { get; set; }
    }
}
