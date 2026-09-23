using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Minio
{
    public sealed class MinioOptions
    {
        public const string SectionName = "Minio";

        public string InternalEndpoint { get; init; } = string.Empty;
        public string ExternalEndpoint { get; init; } = string.Empty;
        public string AccessKey { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public string BucketName { get; init; } = "movie-media";
        public string Region { get; init; } = "us-east-1";
        public bool InternalUseSsl { get; init; }
        public bool ExternalUseSsl { get; init; }
    }
}
