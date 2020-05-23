using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class RequestHandler
    {
        public async Task HandleRequest<TRequest>(HttpContext context)
            where TRequest : AbstractRequest, new()
        {
            try
            {               

                try
                {
                    var response = await new TRequest().ExecuteAsync(context);

                    if (response == null)
                    {
                        var error = $"Response return by {typeof(TRequest).Name} was null.";

                        await ExecuteErrorAsync(context.Response, error);

                        return;
                    }

                    context.Response.StatusCode = (int)response.StatusCode;

                    if (response is IEmptyResponse)
                    {
                        return;
                    }

                    if (response is IDownloadResponse downloadResponse)
                    {
                        await ExecuteDownloadAsync(context.Response, downloadResponse);
                    }
                    else if (response is IJsonResponse jsonResponse)
                    {
                        await ExecuteJsonResponseAsync(context.Response, jsonResponse);
                    }
                    else
                    {
                        var error = $"Unable to get response handler for {response.GetType().Name}";

                        await ExecuteErrorAsync(context.Response, error);
                    }
                }
                catch (Exception exception)
                {
                    await ExecuteErrorAsync(context.Response, exception, "Unable to process request via request handler");
                }
            }
            catch (Exception exception)
            {
                // This is the absolute last resort, it's failed somehow so just try and write a really basic response manually.

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync($"Request handler failed entirely: {exception.Message}");
            }
        }

        private async Task ExecuteErrorAsync(HttpResponse response, Exception exception, string customError = null)
        {
            await ExecuteJsonResponseAsync(response, new ServerErrorResponse(exception, customError));
        }

        private async Task ExecuteErrorAsync(HttpResponse response, string error)
        {
            await ExecuteJsonResponseAsync(response, new ServerErrorResponse(error));
        }

        private static async Task ExecuteDownloadAsync(HttpResponse httpResponse, IDownloadResponse downloadResponse)
        {
            httpResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{downloadResponse.FileName}\"");

            await downloadResponse.Stream.CopyToAsync(httpResponse.Body);
        }

        private static async Task ExecuteJsonResponseAsync(HttpResponse httpResponse, IJsonResponse jsonResponse)
        {
            httpResponse.ContentType = "application/json";

            await httpResponse.WriteAsync(jsonResponse.Json);
        }
    }
}
