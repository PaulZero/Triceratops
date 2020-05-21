using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Core.Enums;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Libraries.Http.Core
{
    public class CoreHttpClient : IPlatformHttpClient
    {
        protected string BaseUrl { get; private set; }

        protected ILogger Logger { get; }

        public CoreHttpClient(ILogger logger)
        {
            Logger = logger;
        }

        public void SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task<DownloadStream> DownloadAsync(string relativeUrl)
        {
            var request = CreateRequest(relativeUrl, AllowedHttpMethod.Get);
            var response = await request.GetResponseAsync();

            if (!response.Headers.AllKeys.Contains("Content-Disposition"))
            {
                throw new Exception($"No Content-Disposition header was received from {relativeUrl}");
            }

            var disposition = response.Headers.Get("Content-Disposition");
            var fileName = disposition.Split(';')
                .Select(s => s.Trim())
                .FirstOrDefault(s => s.StartsWith("filename="))
                ?.Replace("filename=", "") ?? "File.txt";

            using var stream = response.GetResponseStream();
            var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            return new DownloadStream(memoryStream, fileName);
        }

        public async Task<bool> CheckUrlReturnsOkAsync(string relativeUrl)
        {
            try
            {
                var request = CreateRequest(relativeUrl, AllowedHttpMethod.Get);
                var response = await request.GetResponseAsync() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                Logger.LogInformation($"{BaseUrl}{relativeUrl} did not return an OK status code.");

                return false;
            }
        }

        public Task GetAsync(string relativeUrl)
            => SendRequestAsync(relativeUrl, AllowedHttpMethod.Get);

        public Task<T> GetAsync<T>(string relativeUrl)
            => SendRequestAsync<T>(relativeUrl, AllowedHttpMethod.Get);

        public Task PostAsync(string relativeUrl)
            => SendRequestAsync(relativeUrl, AllowedHttpMethod.Post);

        public Task<T> PostAsync<T>(string relativeUrl, object requestBody = null)
            => SendRequestAsync<T>(relativeUrl, AllowedHttpMethod.Post, requestBody);

        public Task UploadAsync(string relativeUrl, Stream stream)
            => SendUploadRequestAsync(relativeUrl, stream);

        public async Task<T> UploadAsync<T>(string relativeUrl, Stream stream)
        {
            var response = await SendUploadRequestAsync(relativeUrl, stream);

            return JsonHelper.Deserialise<T>(response);
        }

        private async Task<string> SendUploadRequestAsync(string relativeUrl, Stream stream)
        {
            try
            {
                var request = CreateRequest(relativeUrl, AllowedHttpMethod.Post);

                using var requestStream = await request.GetRequestStreamAsync();

                await stream.CopyToAsync(requestStream);

                using var response = await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var reader = new StreamReader(responseStream);

                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                Logger.LogError($"Failed to upload file to {relativeUrl}: {exception.Message}");

                throw;
            }
        }

        private async Task<T> SendRequestAsync<T>(string relativeUrl, AllowedHttpMethod method, object requestBody = null)
        {
            try
            {
                var responseString = await SendRequestAsync(relativeUrl, method, requestBody);

                return JsonHelper.Deserialise<T>(responseString);
            }
            catch (System.Text.Json.JsonException exception)
            {
                Logger.LogError($"Failed to deserialise response from {relativeUrl} to instance of {typeof(T).Name}: {exception.Message}");

                throw;
            }
        }

        private async Task<string> SendRequestAsync(string relativeUrl, AllowedHttpMethod method, object requestBody = null)
        {
            try
            {
                var request = CreateRequest(relativeUrl, method);

                if (requestBody != null)
                {
                    await WriteRequestBody(request, requestBody);
                }

                using var response = await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var reader = new StreamReader(responseStream);

                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                var stringBuilder = new StringBuilder($"Failed to send {method} to {BaseUrl}{relativeUrl}: ");

                if (exception is WebException webException && webException?.Response is HttpWebResponse response)
                {
                    stringBuilder.Append($"Server returned a status code of {response.StatusCode}");
                }
                else
                {
                    stringBuilder.Append(exception.Message);
                }

                Logger.LogError(stringBuilder.ToString());

                throw;
            }
        }

        private HttpWebRequest CreateRequest(string relativeUrl, AllowedHttpMethod method)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new Exception($"{nameof(CoreHttpClient)}.{nameof(SetBaseUrl)} MUST be called with a valid base URL before using the client.");
            }

            var fullUrl = $"{BaseUrl}{relativeUrl}";

            var request = WebRequest.CreateHttp(fullUrl);

            request.Method = CreateHttpMethodString(method);

            return request;
        }

        private async Task WriteRequestBody(HttpWebRequest request, object requestBody)
        {
            try
            {
                using var stream = await request.GetRequestStreamAsync();
                using var writer = new StreamWriter(stream);

                var requestJson = CreateRequestJson(requestBody);

                await writer.WriteAsync(requestJson);

                request.ContentType = "application/json";
            }
            catch (Exception exception)
            {
                Logger.LogError($"Unable to write request body: {exception.Message}");

                throw;
            }
        }

        private string CreateRequestJson(object requestBody)
        {
            try
            {
                return JsonHelper.Serialise(requestBody);
            }
            catch (Exception exception)
            {
                Logger.LogInformation($"Unable to serialise request body object: {exception.Message}");

                throw;
            }
        }

        private string CreateHttpMethodString(AllowedHttpMethod method)
        {
            switch (method)
            {
                case AllowedHttpMethod.Get:
                    return "GET";

                case AllowedHttpMethod.Post:
                    return "POST";

                default:
                    throw new Exception($"HTTP client does not support method {method}");
            }
        }

        public static IPlatformHttpClient Create(ILogger logger, string baseUrl)
        {
            var client = new CoreHttpClient(logger);

            client.SetBaseUrl(baseUrl);

            return client;
        }
    }
}
