using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Core.Auth;
using MovieApi.Core.Features.Actors.Create;
using MovieApi.Core.Features.Actors.Delete;
using MovieApi.Core.Features.Actors.GetById;
using MovieApi.Core.Features.Actors.GetList;
using MovieApi.Core.Features.Actors.Update;
using MovieApi.Core.Features.Auth;
using MovieApi.Core.Features.Genres.Create;
using MovieApi.Core.Features.Genres.Delete;
using MovieApi.Core.Features.Genres.GetById;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Core.Features.Genres.Update;
using MovieApi.Core.Features.Movies.Create;
using MovieApi.Core.Features.Movies.Delete;
using MovieApi.Core.Features.Movies.GetById;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Features.Movies.Update;
using MovieApi.Core.Features.Reviews.Create;
using MovieApi.Core.Features.Reviews.Delete;
using MovieApi.Core.Features.Reviews.GetById;
using MovieApi.Core.Features.Reviews.GetList;
using MovieApi.Core.Features.Reviews.Update;
using MovieApi.Core.Features.Users;
using MovieApi.Core.Security;
using MovieApi.Web.Configuration;
using MovieApi.Web.Endpoints;
using MovieApi.Web.Security;
using System.Runtime.CompilerServices;
using System.Text;

namespace MovieApi.Web
{
    public static class DependencyInjection
    {
        public static IEndpointRouteBuilder AddMapEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapMovieEndpoints();
            builder.MapGenresEndpoints();
            builder.MapActorsEndpoints();
            builder.MapReviewEndpoints();
            builder.MapAuthEndpoints();
            builder.MapUserEndpoints();

            return builder;
        }

        public static IServiceCollection AddJwtOptions(this IServiceCollection services)
        {
            services
                .AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.Issuer),
                    "Jwt:Issuer должен быть задан.")
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.Audience),
                    "Jwt:Audience должен быть задан.")
                .Validate(
                    options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32,
                    "Jwt:SigningKey должен содержать минимум 32 байта.")
                .Validate(
                    options => options.LifetimeMinutes is > 0 and <= 1440,
                    "Jwt:LifetimeMinutes должен быть в диапазоне от 1 до 1440.")
                .ValidateOnStart();

            services.AddSingleton<IAccessTokenService, JwtAccessTokenService>();

            return services;
        }

        public static IServiceCollection AddJwtBearer(
            this IServiceCollection services,
            ConfigurationManager configuration)
        {
            var jwtOptions = configuration
                .GetRequiredSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration is required.");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                        RoleClaimType = MovieApiClaimTypes.Role
                    };
                });

            return services;
        }

        public static IServiceCollection AddCurrentUser(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, HttpCurrentUser>();

            return services;
        }
    }
}
