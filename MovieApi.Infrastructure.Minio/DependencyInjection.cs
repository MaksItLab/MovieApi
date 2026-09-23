using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MovieApi.Core.Interfaces;
using MovieApi.Infrastructure.Minio.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Minio
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMinioInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(MinioOptions.SectionName);
            var options = new MinioOptions
            {
                InternalEndpoint = section["InternalEndpoint"] ?? string.Empty,
                ExternalEndpoint = section["ExternalEndpoint"] ?? string.Empty,
                AccessKey = section["AccessKey"] ?? string.Empty,
                SecretKey = section["SecretKey"] ?? string.Empty,
                BucketName = section["BucketName"] ?? "movie-media",
                Region = section["Region"] ?? "us-east-1",
                InternalUseSsl = bool.TryParse(section["InternalUseSsl"], out var internalSsl) && internalSsl,
                ExternalUseSsl = bool.TryParse(section["ExternalUseSsl"], out var externalSsl) && externalSsl
            };

            Validate(options);

            var internalClient = CreateClient(
                options.InternalEndpoint,
                options.InternalUseSsl,
                options);

            var externalClient = CreateClient(
                options.ExternalEndpoint,
                options.ExternalUseSsl,
                options);

            services.AddSingleton(options);
            services.AddSingleton(new MinioClients(internalClient, externalClient));
            services.AddScoped<IObjectStorage, MinioObjectStorage>();

            return services;
        }

        private static IMinioClient CreateClient(
            string endpoint,
            bool useSsl,
            MinioOptions options)
        {
            return new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithRegion(options.Region)
                .WithSSL(useSsl)
                .Build();
        }

        private static void Validate(MinioOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.InternalEndpoint)
                || string.IsNullOrWhiteSpace(options.ExternalEndpoint)
                || string.IsNullOrWhiteSpace(options.AccessKey)
                || string.IsNullOrWhiteSpace(options.SecretKey)
                || string.IsNullOrWhiteSpace(options.BucketName)
                || string.IsNullOrWhiteSpace(options.Region))
            {
                throw new InvalidOperationException(
                    "Minio settings are incomplete.");
            }
        }
    }
}
