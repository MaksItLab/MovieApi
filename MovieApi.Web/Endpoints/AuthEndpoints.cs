using MovieApi.Contracts.Auth;
using MovieApi.Core.Features.Auth;
using MovieApi.Web.Endpoints.Common;

namespace MovieApi.Web.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth");

            group.MapPost("/register", Register);
            group.MapPost("/login", Login);

            return app;
        }

        private static async Task<IResult> Register(
        RegisterRequest request,
        RegisterHandler handler,
        CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);
            return ResultMapper.ToHttpResult(result);
        }

        private static async Task<IResult> Login(
        LoginRequest request,
        LoginHandler handler,
        CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);
            return ResultMapper.ToHttpResult(result);
        }
    }
}
