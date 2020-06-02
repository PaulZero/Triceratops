using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.DockerService.Models
{
    public class TemporaryStorageContainer : TemporaryContainer, IStorageContainer
    {
        private readonly IPlatformHttpClient _httpClient;

        public TemporaryStorageContainer(IPlatformHttpClient httpClient, Guid serverId, string dockerContainerId)
            : base(dockerContainerId, serverId)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckContainerAliveAsync()
        {
            try
            {
                return await _httpClient.CheckUrlReturnsOkAsync("/verify");
            }
            catch
            {
                return false;
            }
        }

        public async Task<DownloadStream> DownloadFileAsync(string relativeFilePath)
        {
            RefreshLastAccessed();

            return await _httpClient.DownloadAsync($"/download/{HashHelper.CreateHash(relativeFilePath)}");
        }

        public async Task<DownloadStream> DownloadServerZipAsync()
        {
            RefreshLastAccessed();

            return await _httpClient.DownloadAsync("/download-zip");
        }

        public async Task<IEnumerable<VolumeDirectory>> GetServerStorageAsync()
        {
            RefreshLastAccessed();

            return await _httpClient.GetAsync<IEnumerable<VolumeDirectory>>("/");
        }

        public async Task DeleteFileAsync(string relativeFilePath)
        {
            RefreshLastAccessed();

            await _httpClient.PostAsync($"/delete/{HashHelper.CreateHash(relativeFilePath)}");
        }

        public async Task UploadFileAsync(string relativeFilePath, Stream fileStream)
        {
            RefreshLastAccessed();

            await _httpClient.UploadAsync($"/upload/{HashHelper.CreateHash(relativeFilePath)}", fileStream);
        }
    }
}
