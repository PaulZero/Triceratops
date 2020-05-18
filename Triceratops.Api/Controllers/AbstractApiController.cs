using Microsoft.AspNetCore.Mvc;
using Triceratops.Libraries.Http.Api.ResponseModels;

namespace Triceratops.Api.Controllers
{
    public abstract class AbstractApiController : Controller
    {
        protected IActionResult ViewModel<T>(T viewModel)
            where T : class, new()
        {
            return Json(new ApiModelResponse<T>(viewModel));
        }

        protected ApiResponse Success(string message)
        {
            return new ApiResponse(true, message);
        }

        protected ApiResponse Error(string error)
        {
            return new ApiResponse(false, error);
        }
    }
}
