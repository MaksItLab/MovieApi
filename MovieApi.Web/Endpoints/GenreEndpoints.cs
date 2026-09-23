using MovieApi.Contracts.Common;
using MovieApi.Contracts.Genres;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Genres.Create;
using MovieApi.Core.Features.Genres.Delete;
using MovieApi.Core.Features.Genres.GetById;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Core.Features.Genres.Update;
using MovieApi.Web.Authorization;
using MovieApi.Web.Endpoints.Common;

namespace MovieApi.Web.Endpoints
{
    public static class GenreEndpoints
    {
        public static IEndpointRouteBuilder MapGenresEndpoints(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/genres", GetGenres);
            app.MapGet("/genres/{id:guid}", GetGenreById)
                .WithName("GetGenreById");
            app.MapPost("/genres", CreateGenre)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);
            app.MapPut("/genres/{id:guid}", UpdateGenre)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);
            app.MapDelete("/genres/{id:guid}", DeleteGenre)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            return app;
        }

        private static async Task<IResult> GetGenres(
            [AsParameters] GetGenresRequest request,
            GetGenresHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> GetGenreById(
            Guid id,
            GetGenreByIdHandler handler,
            CancellationToken token)
        {
            var result = await handler.HandleAsync(id, token);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!); ;
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> CreateGenre(
            CreateGenreRequest request,
            CreateGenreHandler handler,
            CancellationToken token)
        {
            var result = await handler.HandleAsync(request, token);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return ResultMapper.ToCreatedResult(
                result,
                "GetGenreById",
                genre => new { id = genre.Id });
        }

        private static async Task<IResult> UpdateGenre(
            Guid id,
            UpdateGenreRequest request,
            UpdateGenreHandler handler,
            CancellationToken token)
        {
            var result = await handler.HandleAsync(id, request, token);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> DeleteGenre(
            Guid id,
            DeleteGenreHandler handler,
            CancellationToken token)
        {
            var result = await handler.HandleAsync(id, token);

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
