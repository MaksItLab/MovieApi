using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MovieApi.Core.Interfaces;
using MovieApi.Tests.Fakes;

namespace MovieApi.Tests.Infrastructure
{
    public sealed class MovieApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(TestJwt.Configuration);
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IMovieRepository>();
                services.RemoveAll<IGenreRepository>();
                services.RemoveAll<IActorRepository>();

                services.AddScoped<IMovieRepository, FakeMovieRepository>();
                services.AddScoped<IGenreRepository, FakeGenreRepository>();
                services.AddScoped<IActorRepository, FakeActorRepository>();
            });
        }
    }
}
