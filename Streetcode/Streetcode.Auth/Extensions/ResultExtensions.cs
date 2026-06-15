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

            var error = result.Errors.FirstOrDefault();

            return controller.BadRequest(error?.Message ?? "Something went wrong");
        }
    }
}
