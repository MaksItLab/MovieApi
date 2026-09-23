using Serilog;

namespace MovieApi.Web.Configuration
{
    public static class LoggingExtensions
    {
        public static void AddMovieApiLogging(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "MovieApi");
            });
        }
    }
}
