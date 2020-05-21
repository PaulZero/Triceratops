using System.Collections.Generic;

namespace Triceratops.VolumeInspector._Libraries
{
    public class VolumeDirectory
    {
        public string Name { get; set; }

        public string RelativePath { get; set; }

        public IEnumerable<VolumeFile> Files { get; set; }

        public IEnumerable<VolumeDirectory> Directories { get; set; }
    }
}
