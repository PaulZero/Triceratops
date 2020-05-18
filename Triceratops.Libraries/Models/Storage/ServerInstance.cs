namespace Triceratops.Libraries.Models.Storage
{
    public class ServerInstance
    {
        public string ServerSlug { get; set; }

        public ServerDirectory[] Volumes { get; set; }
    }
}
