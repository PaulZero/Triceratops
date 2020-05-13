using Microsoft.AspNetCore.Mvc;
using Triceratops.Libraries.Models.Api.Response;

namespace Triceratops.Api.Controllers
{
    public abstract class AbstractApiController : Controller
    {
        protected IActionResult ViewModel<T>(T viewModel)
            where T : class, new()
        {
            return Json(new ApiModelResponse<T>(viewModel));
        }

        protected IActionResult Success(string message)
        {
            return Json(new ApiResponse(true, message));
        }

        protected IActionResult Error(string error)
        {
            return Json(new ApiResponse(false, error));
        }
    }
}
