using AgileBoard.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace AgileBoard.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Use ToActionResult only for failed results. For success, handle manually.");

            return result.ErrorType switch
            {
                "NotFound" => controller.NotFound(result.ErrorMessage),
                "BadRequest" => controller.BadRequest(result.ErrorMessage),
                "Unauthorized" => controller.Unauthorized(result.ErrorMessage),
                "Conflict" => controller.Conflict(result.ErrorMessage),
                _ => controller.StatusCode(500, result.ErrorMessage)
            };
        }

        public static IActionResult ToActionResult(this Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Use ToActionResult only for failed results. For success, handle manually.");

            return result.ErrorType switch
            {
                "NotFound" => controller.NotFound(result.ErrorMessage),
                "BadRequest" => controller.BadRequest(result.ErrorMessage),
                "Unauthorized" => controller.Unauthorized(result.ErrorMessage),
                "Conflict" => controller.Conflict(result.ErrorMessage),
                _ => controller.StatusCode(500, result.ErrorMessage)
            };
        }

        public static IActionResult? ToActionResultIfFailed<T>(this Result<T> result, ControllerBase controller)
        {
            return result.IsSuccess ? null : result.ToActionResult(controller);
        }

        public static IActionResult? ToActionResultIfFailed(this Result result, ControllerBase controller)
        {
            return result.IsSuccess ? null : result.ToActionResult(controller);
        }
    }
}