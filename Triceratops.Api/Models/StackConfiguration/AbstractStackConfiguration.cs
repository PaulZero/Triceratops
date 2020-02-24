using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration
{
    public abstract class AbstractStackConfiguration : IStackConfiguration
    {
        const string CONTAINER_PREFIX_SEPARATOR = "__";
        protected string _stackPrefix;
        protected IDockerService _dockerService;
        protected IList<string> _containerIds = new List<string>();
        protected IList<string> _imageNames = new List<string>();

        public AbstractStackConfiguration(IDockerService dockerService, string stackPrefix = "")
        {
            _dockerService = dockerService;
            _stackPrefix = stackPrefix;
        }

        public virtual async Task DownloadImages()
        {
            foreach (var imageName in _imageNames)
            {
                var imageSpec = GetImageSpecFromImageName(imageName);
                await _dockerService.DownloadImage(imageSpec.Item1, imageSpec.Item2);
            }
        }

        public virtual async Task Build()
        {
            await DownloadImages();
        }

        public virtual async Task Start()
        {
            var starts = new List<Task>();

            foreach (var containerId in _containerIds) {
                starts.Add(_dockerService.RunContainer(containerId));
            }

            await Task.WhenAll(starts);
        }

        public virtual async Task Stop()
        {
            var stops = new List<Task>();

            foreach (var containerId in _containerIds)
            {
                stops.Add(_dockerService.StopContainer(containerId));
            }

            await Task.WhenAll(stops);
        }

        public virtual async Task Destroy()
        {
            var deletions = new List<Task>();

            foreach (var containerId in _containerIds)
            {
                deletions.Add(_dockerService.DeleteContainer(containerId));
            }

            await Task.WhenAll(deletions);
        }

        protected static Tuple<string, string> GetImageSpecFromImageName(string imageName)
        {
            if (imageName.Contains(':'))
            {
                var split = imageName.Split(':');
                return new Tuple<string, string>(split[0], split[1]);
            }
            else
            {
                return new Tuple<string, string>(imageName, "latest");
            }
        }

        protected string WithPrefix(string containerName)
        {
            return _stackPrefix + CONTAINER_PREFIX_SEPARATOR + containerName;
        }
    }
}
