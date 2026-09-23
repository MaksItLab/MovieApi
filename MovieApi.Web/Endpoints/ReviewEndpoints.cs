using MovieApi.Contracts.Common;
using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Reviews.Create;
using MovieApi.Core.Features.Reviews.Delete;
using MovieApi.Core.Features.Reviews.GetById;
using MovieApi.Core.Features.Reviews.GetList;
using MovieApi.Core.Features.Reviews.Update;
using MovieApi.Web.Endpoints.Common;

namespace MovieApi.Web.Endpoints
{
    public static class ReviewEndpoints
    {
        public static IEndpointRouteBuilder MapReviewEndpoints(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/movies/{movieId:guid}/reviews", GetMovieReviews)
                .WithName("GetReviewById");
            app.MapPost("/movies/{movieId:guid}/reviews", CreateReview)
                .RequireAuthorization();
            app.MapGet("/reviews/{id:guid}", GetReviewById);
            app.MapPut("/reviews/{id:guid}", UpdateReview)
                .RequireAuthorization();
            app.MapDelete("/reviews/{id:guid}", DeleteReview)
                .RequireAuthorization();

            return app;
        }

        private static async Task<IResult> GetMovieReviews(
            Guid movieId,
            GetMovieReviewsHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(movieId, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> GetReviewById(
            Guid id,
            GetReviewByIdHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> CreateReview(
            Guid movieId,
            CreateReviewRequest request,
            CreateReviewHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(movieId, request, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return ResultMapper.ToCreatedResult(
                result,
                "GetReviewById",
                review => new { id = review.Id });
        }

        private static async Task<IResult> UpdateReview(
            Guid id,
            UpdateReviewRequest request,
            UpdateReviewHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> DeleteReview(
            Guid id,
            DeleteReviewHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.NoContent();
        }
    }
}
