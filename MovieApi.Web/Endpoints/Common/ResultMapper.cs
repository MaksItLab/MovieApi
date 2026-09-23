using MovieApi.Contracts.Common;
using MovieApi.Core.Common;

namespace MovieApi.Web.Endpoints.Common
{
    public static class ResultMapper
    {
        public static IResult ToHttpResult<T>(OperationResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ToErrorResult(result.ErrorType!.Value, result.ErrorMessage!);
        }

        public static IResult ToNoContentResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            return ToErrorResult(result.ErrorType!.Value, result.ErrorMessage!);
        }

        public static IResult ToCreatedResult<T>(
            OperationResult<T> result,
            string route,
            Func<T, object> routeValues)
        {
            if (result.IsSuccess)
            {
                return Results.CreatedAtRoute(route, routeValues(result.Value!), result.Value);
            }

            return ToErrorResult(result.ErrorType!.Value, result.ErrorMessage!);
        }

        public static IResult ToErrorResult(
            OperationErrorType? errorType,
            string message)
        {
            var error = new ErrorResponse(message);

            return errorType switch
            {
                OperationErrorType.Validation => Results.BadRequest(error),
                OperationErrorType.NotFound => Results.NotFound(error),
                OperationErrorType.Conflict => Results.Conflict(error),
                OperationErrorType.Unauthorized => Results.Json(
                    error,
                    statusCode: StatusCodes.Status401Unauthorized),
                OperationErrorType.Unavailable => Results.Json(
                    new ErrorResponse(message),
                    statusCode: StatusCodes.Status503ServiceUnavailable),

                _ => Results.BadRequest(error),
            };
        }
    }
}
