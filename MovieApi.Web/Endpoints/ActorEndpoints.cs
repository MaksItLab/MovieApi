using MovieApi.Contracts.Actors;
using MovieApi.Contracts.Common;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Actors.Create;
using MovieApi.Core.Features.Actors.Delete;
using MovieApi.Core.Features.Actors.GetById;
using MovieApi.Core.Features.Actors.GetList;
using MovieApi.Core.Features.Actors.Update;
using MovieApi.Web.Authorization;
using MovieApi.Web.Endpoints.Common;

namespace MovieApi.Web.Endpoints
{
    public static class ActorEndpoints
    {
        public static IEndpointRouteBuilder MapActorsEndpoints(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/actors", GetActors);
            app.MapGet("/actors/{id:guid}", GetActorById)
                .WithName("GetActorById");
            app.MapPost("/actors", CreateActor)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);
            app.MapPut("/actors/{id:guid}", UpdateActor)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);
            app.MapDelete("/actors/{id:guid}", DeleteActor)
                .RequireAuthorization(AuthorizationPolicies.ManageCatalog);

            app.MapGet("/debug/error", () =>
            {
                throw new InvalidOperationException("Test exception.");
            });

            return app;
        }

        private static async Task<IResult> GetActors(
            GetActorsHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return Results.Ok(result.Value);
        }

        private static async Task<IResult> GetActorById(
            Guid id,
            GetActorByIdHandler handler,
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

        private static async Task<IResult> CreateActor(
            CreateActorRequest request,
            CreateActorHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return ResultMapper.ToErrorResult(
                    result.ErrorType,
                    result.ErrorMessage!);
            }

            return ResultMapper.ToCreatedResult(
                result,
                "GetActorById",
                actor => new { id = actor.Id });
        }

        private static async Task<IResult> UpdateActor(
            Guid id,
            UpdateActorRequest request,
            UpdateActorHandler handler,
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

        private static async Task<IResult> DeleteActor(
            Guid id,
            DeleteActorHandler handler,
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
