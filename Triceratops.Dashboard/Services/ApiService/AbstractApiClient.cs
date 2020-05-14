using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Api.Response;

namespace Triceratops.Dashboard.Services.ApiService
{
    public class AbstractApiClient
    {
        private readonly string _baseUrl;

        public AbstractApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public Task<ApiResponse> GetAsync(string relativeUrl)
        {
            return GetAsync<ApiResponse>(relativeUrl);
        }

        public Task<T> GetAsync<T>(string relativeUrl)
            where T : class, new()
        {
            return SendRequestAsync<T>(relativeUrl, "GET");
        }

        public Task<ApiResponse> PostAsync(string relativeUrl, object model = null)
        {
            return PostAsync<ApiResponse>(relativeUrl, model);
        }

        public Task<T> PostAsync<T>(string relativeUrl, object model = null)
            where T : class, new()
        {
            return SendRequestAsync<T>(relativeUrl, "POST", model);
        }

        private async Task<T> SendRequestAsync<T>(string relativeUrl, string method, object model = null)
            where T : class, new()
        {
            try
            {
                var request = WebRequest.CreateHttp($"{_baseUrl}{relativeUrl}");
                request.Method = method;

                if (model != null && method == "POST")
                {
                    request.ContentType = "application/json";

                    using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
                    {
                        var json = JsonConvert.SerializeObject(model);
                        await writer.WriteAsync(json);
                    }
                }

                using (var response = await request.GetResponseAsync())                
                {
                    var responseBody = await ReadResponseBodyAsync(response);

                    if (typeof(T) == typeof(ApiResponse))
                    {
                        var apiResponse = JsonConvert.DeserializeObject<T>(responseBody);

                        return apiResponse ?? throw CreateExceptionForResponse(responseBody, relativeUrl, method);
                    }

                    var viewModelResponse = JsonConvert.DeserializeObject<ApiModelResponse<T>>(responseBody);

                    return viewModelResponse?.Model ?? throw CreateExceptionForResponse(responseBody, relativeUrl, method);
                }
            } 
            catch (WebException exception)
            {
                var responseBody = await ReadResponseBodyAsync(exception.Response);

                throw CreateExceptionForResponse(responseBody, relativeUrl, method);
            }
        }

        private Exception CreateExceptionForResponse(string responseBody, string relativeUrl, string method)
        {
            var requestSummary = $"{method} - {relativeUrl}:";

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new Exception($"{requestSummary} Empty response received from server.");
            }

            var errorResponse  = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

            if (!string.IsNullOrEmpty(errorResponse?.Message))
            {
                throw new Exception($"{requestSummary} {errorResponse.Message}");
            }

            throw new Exception($"{requestSummary} Malformed response received from server.");
        }

        private async Task<string> ReadResponseBodyAsync(WebResponse response)
        {
            try
            {
                if (response == null)
                {
                    return "";
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
