using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IObjectStorage
    {
        Task PutAsync(ObjectToStore objectToStore, CancellationToken cancellationToken);

        Task<TemporaryObjectUrl?> CreateReadUrlAsync(
            string objectKey,
            TimeSpan lifetime,
            CancellationToken cancellationToken);

        Task DeleteIfExistsAsync(string objectKey, CancellationToken cancellationToken);
    }

    public sealed record ObjectToStore(
        string ObjectKey,
        string ContentType,
        long Length,
        Stream Content);

    public sealed record TemporaryObjectUrl(
        string Url,
        DateTimeOffset ExpiresAtUtc);

    public sealed class ObjectStorageUnavailableException : Exception
    {
        public ObjectStorageUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
