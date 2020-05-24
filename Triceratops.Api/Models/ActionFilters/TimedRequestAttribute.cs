using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Api.Models.ActionFilters
{
    public class TimedRequestAttribute : ActionFilterAttribute
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is ITimedResponse timedResponse)
            {
                timedResponse.Duration = _stopwatch.Elapsed;
            }
        }
    }
}
