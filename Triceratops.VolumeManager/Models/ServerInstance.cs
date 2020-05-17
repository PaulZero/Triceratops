namespace Triceratops.VolumeManager.Models
{
    public class ServerInstance
    {
        public string Name { get; set; }

        public ServerDirectory[] Directories { get; set; }
    }
}
