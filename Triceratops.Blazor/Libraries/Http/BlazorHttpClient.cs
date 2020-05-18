using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Triceratops.Libraries;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Blazor.Libraries.Http
{
    public class BlazorHttpClient : IPlatformHttpClient
    {
        protected string BaseUrl { get; private set; }

        protected ILogger Logger { get; }

        protected HttpClient HttpClient { get; }

        public BlazorHttpClient(ILogger logger, HttpClient httpClient)
        {
            Logger = logger;
            HttpClient = httpClient;
        }

        public void SetBaseUrl(string baseUrl)
        {
            if (baseUrl == TriceratopsConstants.InternalApiUrl)
            {
                baseUrl = TriceratopsConstants.BlasphemousApiUrl;
            }

            BaseUrl = baseUrl;
        }

        public Task<T> GetAsync<T>(string relativeUrl)
            => HttpClient.GetFromJsonAsync<T>(CreateFullUrl(relativeUrl));

        public async Task<T> PostAsync<T>(string relativeUrl, object requestBody)
        {
            var response = await HttpClient.PostAsync(CreateFullUrl(relativeUrl), JsonContent.Create(requestBody));

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<FileDownloadResponse> DownloadAsync(string relativeUrl)
        {
            var response = await HttpClient.GetAsync(CreateFullUrl(relativeUrl));
            var content = response.Content;

            if (content is StreamContent streamContent)
            {
                var disposition = response.Headers.GetValues("Content-Disposition").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(disposition))
                {
                    throw new Exception("Missing Content-Disposition header from download response.");
                }

                var fileName = disposition.Split(';')
                    .Select(s => s.Trim())
                    .FirstOrDefault(s => s.StartsWith("filename="))
                    ?.Replace("filename=", "") ?? "File.txt";

                return new FileDownloadResponse(fileName, await streamContent.ReadAsStreamAsync());
            }

            throw new Exception($"Expected StreamContent from download, got {content?.GetType().Name ?? "null"} instead");
        }

        public Task UploadAsync(string relativeUrl, Stream stream)
            => HttpClient.PostAsync(CreateFullUrl(relativeUrl), new StreamContent(stream));
        

        private string CreateFullUrl(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new Exception($"{nameof(BlazorHttpClient)}.{nameof(SetBaseUrl)} MUST be called with a valid base URL before using the client.");
            }

            return $"{BaseUrl}{relativeUrl}";
        }
    }
}
