using MovieApi.Core.Features.Users;
using MovieApi.Web.Endpoints.Common;

namespace MovieApi.Web.Endpoints
{
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder app)
        {
            app.MapGet("/users/me", GetMyProfile)
                .RequireAuthorization()
                .WithName("GetMyProfile")
                .WithTags("Users");

            return app;
        }

        private static async Task<IResult> GetMyProfile(
            GetMyProfileHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(cancellationToken);

            return ResultMapper.ToHttpResult(result);
        }
    }
}
