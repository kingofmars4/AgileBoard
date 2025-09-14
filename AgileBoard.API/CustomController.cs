using AgileBoard.API.Extensions;
using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AgileBoard.API
{
    [ApiController]
    public abstract class CustomController : ControllerBase
    {
        protected async Task<IActionResult?> CheckAuthorizationAsync<T>(
            Func<int, Task<Result<T>>> authCheck, 
            string errorMessage = Messages.Authorization.AccessDenied)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await authCheck(currentUserId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());

            if (authResult.Data is bool canAccess && !canAccess)
                return StatusCode(403, errorMessage);

            return null;
        }

        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

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