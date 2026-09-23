using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApi.Core.Interfaces;
using MovieApi.Infrastructure.Postgres.Persistence;
using MovieApi.Infrastructure.Postgres.Repositories;

namespace MovieApi.Infrastructure.Postgres
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<MovieDbContext>(options =>
            options.UseNpgsql(connectionString));

            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IActorRepository, ActorRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
