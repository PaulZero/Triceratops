using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Libraries.Http.Core
{
    public interface IPlatformHttpClient
    {
        void SetBaseUrl(string baseUrl);

        Task<T> GetAsync<T>(string relativeUrl);

        Task<T> PostAsync<T>(string relativeUrl, object requestBody);

        Task UploadAsync(string relativeUrl, Stream stream);

        Task<FileDownloadResponse> DownloadAsync(string relativeUrl);
    }
}
