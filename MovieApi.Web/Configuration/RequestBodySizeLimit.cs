using Microsoft.AspNetCore.Http.Metadata;

namespace MovieApi.Web.Configuration
{
    public sealed class RequestBodySizeLimit : IRequestSizeLimitMetadata
    {
        public RequestBodySizeLimit(long maxRequestBodySize)
        {
            MaxRequestBodySize = maxRequestBodySize;
        }

        public long? MaxRequestBodySize { get; }
    }
}
