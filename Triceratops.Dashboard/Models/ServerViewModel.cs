using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.Models;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Models
{
    public class ServerViewModel
    {
        public Guid Id { get; }

        public string Name { get; }

        public string Slug { get; }

        public string ServerType { get; }

        public bool IsRunning { get; }

        public IEnumerable<ContainerBasicDetails> Containers { get; } = new ContainerBasicDetails[0];

        public IEnumerable<VolumeDirectory> Volumes { get; } = new VolumeDirectory[0];

        public bool HasContainers => Containers?.Any() ?? false;

        public bool HasVolumes => Volumes?.Any() ?? false;

        public ServerViewModel(ServerBasicDetails basicDetails)
        {
            Id = basicDetails.ServerId;
            Name = basicDetails.Name;
            Slug = basicDetails.Slug;
            IsRunning = basicDetails.IsRunning;

            if (basicDetails is ServerExtendedDetails extendedDetails)
            {
                Containers = extendedDetails.Containers;
            }
        }

        public ServerViewModel(ServerBasicDetails basicDetails, IEnumerable<VolumeDirectory> volumes)
            : this(basicDetails)
        {
            Volumes = volumes;
        }
    }
}
