using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Core.Enums;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Libraries.Http.Core
{
    public abstract class AbstractHttpClient
    {
        protected string BaseUrl { get; }

        protected ILogger Logger { get; }

        public AbstractHttpClient(string baseUrl, ILogger logger)
        {
            BaseUrl = baseUrl;
            Logger = logger;
        }

        protected async Task UploadAsync(string relativeUrl, Stream stream)
        {
            try
            {
                var request = CreateRequest(relativeUrl, AllowedHttpMethod.Post);

                using var requestStream = await request.GetRequestStreamAsync();

                await stream.CopyToAsync(requestStream);

                using var response = await request.GetResponseAsync();
            }
            catch (Exception exception)
            {
                Logger.LogError($"Failed to upload file to {relativeUrl}: {exception.Message}");
            }
        }

        protected async Task<FileDownloadResponse> DownloadAsync(string relativeUrl)
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

            return new FileDownloadResponse(fileName, memoryStream);
        }

        protected Task<string> GetAsync(string relativeUrl, object requestBody = null)
            => SendRequestAsync(relativeUrl, AllowedHttpMethod.Get, requestBody);

        protected Task<T> GetAsync<T>(string relativeUrl, object requestBody = null)
            => SendRequestAsync<T>(relativeUrl, AllowedHttpMethod.Get, requestBody);

        protected Task<string> PostAsync(string relativeUrl, object requestBody = null)
            => SendRequestAsync(relativeUrl, AllowedHttpMethod.Post, requestBody);

        protected Task<T> PostAsync<T>(string relativeUrl, object requestBody = null)
            => SendRequestAsync<T>(relativeUrl, AllowedHttpMethod.Post, requestBody);

        private async Task<T> SendRequestAsync<T>(string relativeUrl, AllowedHttpMethod method, object requestBody = null)
        {
            try
            {
                var responseString = await SendRequestAsync(relativeUrl, method, requestBody);

                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (JsonException exception)
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
                var stringBuilder = new StringBuilder($"Failed to send {method} to {relativeUrl}: ");

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
                return JsonConvert.SerializeObject(requestBody);
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
    }
}
