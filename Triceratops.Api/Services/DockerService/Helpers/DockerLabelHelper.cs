using MongoDB.Bson.Serialization.Conventions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DockerService.Models
{
    public static class DockerLabelHelper
    {
        public const string LabelNamespace = "com.paulzero.triceratops";

        public static string CreatedByTriceratopsLabel { get; } = $"{LabelNamespace}.created-by-triceratops";

        public static string TemporaryContainerLabel { get; } = $"{LabelNamespace}.temporary-container";

        public static string TriceratopsServerId { get; } = $"{LabelNamespace}.server-id";

        public static string TriceratopsContainerId { get; } = $"{LabelNamespace}.container-id";

        public static string TriceratopsVolumeId { get; } = $"{LabelNamespace}.volume-id";

        public static IDictionary<string, string> CreateBaseLabels(params (string, string)[] additionalLabels)
        {
            var labels = new Dictionary<string, string>
            {
                [CreatedByTriceratopsLabel] = "true",
            };

            if (additionalLabels?.Any() ?? false)
            {
                foreach (var (name, value) in additionalLabels)
                {
                    labels.Add(name, value);
                }
            }

            return labels;
        }

        public static IDictionary<string, string> CreateForTemporaryContainer()
        {
            return CreateBaseLabels(
                (TemporaryContainerLabel, "true")
            );
        }

        public static IDictionary<string, string> CreateForVolume(Container container, Volume volume)
        {
            return CreateBaseLabels(
                (TriceratopsServerId, container.ServerId.ToString()),
                (TriceratopsContainerId, container.Id.ToString()),
                (TriceratopsVolumeId, volume.Id.ToString())
            );
        }

        public static IDictionary<string, string> CreateForContainer(Container container)
        {
            return CreateBaseLabels(
                (TriceratopsServerId, container.ServerId.ToString()),
                (TriceratopsContainerId, container.Id.ToString())
            );
        }
    }
}
