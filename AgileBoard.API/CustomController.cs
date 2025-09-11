using AgileBoard.API.Extensions;
using AgileBoard.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace AgileBoard.API
{
    [ApiController]
    public abstract class CustomController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result, Func<T, IActionResult> onSuccess)
        {
            var errorResult = result.ToActionResultIfFailed(this);
            return errorResult ?? onSuccess(result.Data!);
        }

        protected IActionResult HandleResult(Result result, Func<IActionResult> onSuccess)
        {
            var errorResult = result.ToActionResultIfFailed(this);
            return errorResult ?? onSuccess();
        }
    }
}