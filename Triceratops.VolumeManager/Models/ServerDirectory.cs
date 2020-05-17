namespace Triceratops.VolumeManager.Models
{
    public class ServerDirectory
    {
        public string Name { get; set; }

        public ServerDirectory[] Directories { get; set; }

        public ServerFile[] Files { get; set; }
    }
}
