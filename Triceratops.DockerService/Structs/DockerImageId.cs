namespace Triceratops.DockerService.Structs
{
    public struct DockerImageId
    {
        public string Name { get; }

        public string Tag { get; }

        public string FullName => $"{Name}:{Tag}";

        public DockerImageId(string name, string tag)
        {
            Name = name;
            Tag = tag;
        }
    }
}
