using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Streetcode.Auth.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result == null)
            {
                return controller.StatusCode(500, "The operation returned a null result.");
            }

            if (result.IsSuccess)
            {
                return EqualityComparer<T>.Default.Equals(result.Value, default(T))
                     ? controller.NotFound()
                     : controller.Ok(result.Value);
            }

            if (result.Errors.Count > 0 && !string.IsNullOrWhiteSpace(result.Errors[0].Message))
            {
                return controller.BadRequest(result.Errors[0].Message);
            }

            return controller.BadRequest("Something went wrong");
        }
    }
}
