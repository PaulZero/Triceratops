using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Triceratops.Libraries.Http;
using Triceratops.Libraries.Models.Api.Response;

namespace Triceratops.Dashboard.Services.ApiService
{
    public abstract class AbstractApiClient : AbstractHttpClient
    {
        public AbstractApiClient(string baseUrl, ILogger logger) : base(baseUrl, logger)
        {
        }

        public Task<ApiResponse> GetApiAsync(string relativeUrl)
        {
            return GetAsync<ApiResponse>(relativeUrl);
        }

        public Task<ApiResponse> PostApiAsync(string relativeUrl, object model = null)
        {
            return PostAsync<ApiResponse>(relativeUrl, model);
        }

        public async Task<TModel> GetModelAsync<TModel>(string relativeUrl)
            where TModel : class, new()
        {
            try
            {
                var response = await GetAsync<ApiModelResponse<TModel>>(relativeUrl);

                return response.Model;
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to fetch model from {relativeUrl}: {exception.Message}");
            }            
        }

        public async Task<TModel> PostModelAsync<TModel>(string relativeUrl, object model = null)
            where  TModel : class, new()
        {
            try
            {
                var response = await PostAsync<ApiModelResponse<TModel>>(relativeUrl, model);

                return response.Model;
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to fetch model from {relativeUrl}: {exception.Message}");
            } 
        }
    }
}
