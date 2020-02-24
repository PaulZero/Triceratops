using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration
{
    public abstract class AbstractStackConfiguration : IStackConfiguration
    {
        private const string ContainerPrefixSeparator = "__";

        private readonly string _stackPrefix;

        private readonly IDockerService _dockerService;

        private readonly IList<string> _containerIds = new List<string>();

        private readonly IList<string> _imageNames = new List<string>();

        public AbstractStackConfiguration(IDockerService dockerService, string stackPrefix = "")
        {
            _dockerService = dockerService;
            _stackPrefix = stackPrefix;
        }

        public virtual async Task DownloadImagesAsync()
        {
            foreach (var imageName in _imageNames)
            {
                var (image, version) = GetImageSpecFromImageName(imageName);

                await _dockerService.DownloadImageAsync(image, version);
            }
        }

        public virtual async Task BuildAsync()
        {
            await DownloadImagesAsync();
        }

        public virtual async Task StartAsync()
        {
            var starts = _containerIds.Select(id => _dockerService.RunContainerAsync(id));

            await Task.WhenAll(starts);
        }

        public virtual async Task StopAsync()
        {
            var stops = _containerIds.Select(id => _dockerService.StopContainerAsync(id));

            await Task.WhenAll(stops);
        }

        public virtual async Task DestroyAsync()
        {
            var deletions = _containerIds.Select(id => _dockerService.DeleteContainerAsync(id));

            await Task.WhenAll(deletions);
        }

        protected void AddImage(string imageName)
        {
            _imageNames.Add(imageName);
        }

        protected ContainerBuilder CreateContainerBuilder(string imageName, string containerName)
        {
            return new ContainerBuilder(this, imageName, containerName);
        }

        private async Task AddContainerAsync(ContainerBuilder builder)
        {
            _containerIds.Add(await _dockerService.CreateContainerAsync(
                builder.ImageName,
                builder.ContainerName,
                builder.EnvironmentVariables
           ));
        }

        protected string WithPrefix(string containerName)
        {
            return _stackPrefix + ContainerPrefixSeparator + containerName;
        }

        protected static (string Image, string Version) GetImageSpecFromImageName(string imageName)
        {
            if (imageName.Contains(':'))
            {
                var split = imageName.Split(':');

                return (split[0], split[1]);
            }

            return (imageName, "latest");
        }

        protected class ContainerBuilder
        {
            public string ImageName { get; }

            public string ContainerName { get; private set; }

            public IEnumerable<string> EnvironmentVariables => _environmentVariables.Distinct();

            private readonly AbstractStackConfiguration _stackConfig;

            private readonly List<string> _environmentVariables = new List<string>();

            private bool _hasAppliedPrefix;

            private bool _hasCreatedContainer;

            public ContainerBuilder(AbstractStackConfiguration stackConfig, string imageName, string containerName)
            {
                _stackConfig = stackConfig;
                ImageName = imageName;
                ContainerName = containerName;
            }

            public ContainerBuilder UsePrefix()
            {
                if (_hasAppliedPrefix)
                {
                    return this;
                }

                ContainerName = $"{_stackConfig._stackPrefix}{ContainerPrefixSeparator}{ContainerName}";

                _hasAppliedPrefix = true;

                return this;
            }

            public ContainerBuilder WithEnvironmentVariables(params string[] envVariables)
            {
                _environmentVariables.AddRange(envVariables);

                return this;
            }

            public async Task CreateContainerAsync()
            {
                if (_hasCreatedContainer)
                {
                    return;
                }

                await _stackConfig.AddContainerAsync(this);

                _hasCreatedContainer = true;
            }
        }
    }
}
