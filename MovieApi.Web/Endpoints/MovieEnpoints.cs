using Microsoft.AspNetCore.Mvc;
using MovieApi.Contracts.Common;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Features.Movies.Create;
using MovieApi.Core.Features.Movies.Delete;
using MovieApi.Core.Features.Movies.DeletePoster;
using MovieApi.Core.Features.Movies.DeleteVideo;
using MovieApi.Core.Features.Movies.GetById;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Features.Movies.GetPosterAccess;
using MovieApi.Core.Features.Movies.GetVideoAccess;
using MovieApi.Core.Features.Movies.ReplacePoster;
using MovieApi.Core.Features.Movies.Update;
using MovieApi.Core.Features.Movies.UploadPoster;
using MovieApi.Core.Features.Movies.UploadVideo;
using MovieApi.Core.Movies;
using MovieApi.Web.Authorization;
using MovieApi.Web.Configuration;
using MovieApi.Web.Endpoints.Common;
using Serilog;
using Serilog.Events;

namespace MovieApi.Web.Endpoints
{
    public static class MovieEnpoints
    {
        public static IEndpointRouteBuilder MapMovieEndpoints(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/movies", GetMovies);

            app.MapGet("/movies/{id:guid}", GetMovieById)
                .WithName("GetMovieById");

            app.MapPost("/movies", CreateMovie)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapPut("/movies/{id:guid}", UpdateMovie)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapDelete("/movies/{id:guid}", DeleteMovie)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapPost("/movies/{id:guid}/poster", UploadPoster)
                .Accepts<IFormFile>("multipart/form-data")
                .DisableAntiforgery()
                .WithFormOptions(
                    multipartBodyLengthLimit: FileUploadOptions.MaxPosterRequestBodySizeBytes)
                .WithMetadata(new RequestBodySizeLimit(
                    FileUploadOptions.MaxPosterRequestBodySizeBytes))
                .Produces<MoviePosterResponse>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
                .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapGet("/movies/{id:guid}/poster", GetPosterAccess)
                .Produces<MoviePosterAccessResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);

            app.MapPut("/movies/{id:guid}/poster", ReplacePoster)
                .Accepts<IFormFile>("multipart/form-data")
                .DisableAntiforgery()
                .WithFormOptions(
                    multipartBodyLengthLimit: FileUploadOptions.MaxPosterRequestBodySizeBytes)
                .WithMetadata(new RequestBodySizeLimit(
                    FileUploadOptions.MaxPosterRequestBodySizeBytes))
                .Produces<MoviePosterResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapDelete("/movies/{id:guid}/poster", DeletePoster)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapPost("/movies/{id:guid}/video", UploadVideo)
                .Accepts<IFormFile>("multipart/form-data")
                .DisableAntiforgery()
                .WithFormOptions(
                    multipartBodyLengthLimit: FileUploadOptions.MaxVideoRequestBodySizeBytes)
                .WithMetadata(new RequestBodySizeLimit(
                    FileUploadOptions.MaxVideoRequestBodySizeBytes))
                .Produces<MovieVideoResponse>(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
                .Produces<ErrorResponse>(StatusCodes.Status413PayloadTooLarge)
                .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapGet("/movies/{id:guid}/video", GetVideoAccess)
                .Produces<MovieVideoAccessResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);

            app.MapDelete("/movies/{id:guid}/video", DeleteVideo)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            return app;
        }

        private static async Task<IResult> GetMovies(
            [AsParameters] GetMoviesRequest request,
            GetMoviesHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger(nameof(MovieApi.Web));

            var result = await handler.HandleAsync(request, cancellationToken);

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> GetMovieById(
            Guid id,
            GetMovieByIdHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger(nameof(MovieApi.Web));

            var result = await handler.HandleAsync(id, token);

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> CreateMovie(
            CreateMovieRequest request, 
            CreateMovieHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger(nameof(MovieApi.Web));

            var result = await handler.HandleAsync(request, logger, token);
            if (result.IsSuccess)
            {
                logger.LogInformation("Movie {MovieId} was created", result.Value!.Id);
            }

            logger.LogWarning("Movie was not created");

            return ResultMapper.ToCreatedResult(
                result,
                "GetMovieById", 
                movie => new {id = movie.Id});
        }

        private static async Task<IResult> UpdateMovie(
            Guid id,
            UpdateMovieRequest request,
            UpdateMovieHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger(nameof(MovieApi.Web));

            var result = await handler.HandleAsync(id, request, token);

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> DeleteMovie(
            Guid id, 
            DeleteMovieHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger(nameof(MovieApi.Web));

            var result = await handler.HandleAsync(id, token);

            return ResultMapper.ToNoContentResult(result);
        }

        private static async Task<IResult> UploadPoster(
            Guid id,
            IFormFile file,
            UploadMoviePosterHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var upload = new UploadedFile(
                originalFileName,
                file.Length,
                () => file.OpenReadStream());

            var result = await handler.HandleAsync(
                new UploadMoviePosterCommand(id, upload),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToHttpResult(result);
            }

            loggerFactory.CreateLogger("MovieApi.Web")
                .LogInformation(
                    "Poster for movie {MovieId} was uploaded with {ContentType} and {SizeBytes} bytes.",
                    id,
                    result.Value!.ContentType,
                    result.Value.SizeBytes);

            return Results.Created($"/movies/{id}/poster", result.Value);
        }

        private static async Task<IResult> GetPosterAccess(
            Guid id,
            GetMoviePosterAccessHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(id, cancellationToken);

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> ReplacePoster(
            Guid id,
            IFormFile file,
            ReplaceMoviePosterHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var upload = new UploadedFile(
                Path.GetFileName(file.FileName),
                file.Length,
                () => file.OpenReadStream());

            var result = await handler.HandleAsync(
                new ReplaceMoviePosterCommand(id, upload),
                cancellationToken);

            if (result.IsSuccess)
            {
                loggerFactory.CreateLogger("MovieApi.Posters")
                    .LogInformation(
                        "Poster for movie {MovieId} was replaced with {ContentType} and {SizeBytes} bytes.",
                        id,
                        result.Value!.ContentType,
                        result.Value.SizeBytes);
            }

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> DeletePoster(
            Guid id,
            DeleteMoviePosterHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(
                new DeleteMoviePosterCommand(id),
                cancellationToken);

            if (result.IsSuccess)
            {
                loggerFactory.CreateLogger("MovieApi.Posters")
                    .LogInformation(
                        "Poster for movie {MovieId} was deleted.",
                        id);
            }

            return ResultMapper.ToNoContentResult(result);
        }

        private static async Task<IResult> UploadVideo(
            Guid id,
            IFormFile file,
            UploadMovieVideoHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var upload = new UploadedFile(
                Path.GetFileName(file.FileName),
                file.Length,
                () => file.OpenReadStream());

            var result = await handler.HandleAsync(
                new UploadMovieVideoCommand(id, upload),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToHttpResult(result);
            }

            var response = new MovieVideoResponse(
                result.Value!.ContentType,
                result.Value.SizeBytes,
                result.Value.UploadedAt);

            loggerFactory.CreateLogger("MovieApi.Videos")
                .LogInformation(
                    "Video for movie {MovieId} was uploaded with {ContentType} and {SizeBytes} bytes.",
                    id,
                    response.ContentType,
                    response.SizeBytes);

            return Results.Created($"/movies/{id}/video", response);
        }

        private static async Task<IResult> GetVideoAccess(
            Guid id,
            GetMovieVideoAccessHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(id, cancellationToken);

            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> DeleteVideo(
            Guid id,
            DeleteMovieVideoHandler handler,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(
                new DeleteMovieVideoCommand(id),
                cancellationToken);

            if (result.IsSuccess)
            {
                loggerFactory.CreateLogger("MovieApi.Videos")
                    .LogInformation(
                        "Video for movie {MovieId} was deleted.",
                        id);
            }

            return ResultMapper.ToNoContentResult(result);
        }
    }
}
