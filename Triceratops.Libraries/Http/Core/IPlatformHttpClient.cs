using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Libraries.Http.Core
{
    public interface IPlatformHttpClient
    {
        void SetBaseUrl(string baseUrl);

        Task<bool> CheckUrlReturnsOkAsync(string relativeUrl);

        Task GetAsync(string relativeUrl);

        Task<T> GetAsync<T>(string relativeUrl);

        Task PostAsync(string relativeUrl);

        Task<T> PostAsync<T>(string relativeUrl, object requestBody = null);

        Task UploadAsync(string relativeUrl, Stream stream);

        Task<T> UploadAsync<T>(string relativeUrl, Stream stream);

        Task<DownloadStream> DownloadAsync(string relativeUrl);
    }
}
