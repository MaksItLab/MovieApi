using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using MovieApi.Core.Interfaces;
using MovieApi.Infrastructure.Minio.Contracts;


namespace MovieApi.Infrastructure.Minio
{
    public sealed class MinioObjectStorage : IObjectStorage
    {
        private readonly IMinioClient _internalClient;
        private readonly IMinioClient _externalClient;
        private readonly MinioOptions _options;
        private readonly ILogger<MinioObjectStorage> _logger;

        public MinioObjectStorage(
            MinioClients clients,
            MinioOptions options,
            ILogger<MinioObjectStorage> logger)
        {
            _internalClient = clients.Internal;
            _externalClient = clients.External;
            _options = options;
            _logger = logger;
        }

        public async Task PutAsync(
            ObjectToStore objectToStore,
            CancellationToken cancellationToken)
        {
            try
            {
                await EnsureBucketExistsAsync(cancellationToken);

                if (objectToStore.Content.CanSeek)
                {
                    objectToStore.Content.Position = 0;
                }

                var args = new PutObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(objectToStore.ObjectKey)
                    .WithStreamData(objectToStore.Content)
                    .WithObjectSize(objectToStore.Length)
                    .WithContentType(objectToStore.ContentType);

                await _internalClient.PutObjectAsync(args, cancellationToken);

                var storedObject = await _internalClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectToStore.ObjectKey),
                    cancellationToken);

                if (storedObject.Size != objectToStore.Length)
                {
                    throw new InvalidOperationException(
                        "Stored object size does not match the uploaded file size.");
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                throw new ObjectStorageUnavailableException(
                    "Object storage request failed.",
                    exception);
            }
        }

        public async Task DeleteIfExistsAsync(
            string objectKey,
            CancellationToken cancellationToken)
        {
            try
            {
                await _internalClient.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectKey),
                    cancellationToken);
            }
            catch (MinioException exception)
            {
                throw new ObjectStorageUnavailableException(
                    "Object storage request failed.",
                    exception);
            }
        }

        public async Task<TemporaryObjectUrl?> CreateReadUrlAsync(
            string objectKey,
            TimeSpan lifetime,
            CancellationToken cancellationToken)
        {
            if (lifetime <= TimeSpan.Zero
                || lifetime > TimeSpan.FromDays(7))
            {
                throw new ArgumentOutOfRangeException(nameof(lifetime));
            }

            try
            {
                await _internalClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectKey),
                    cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var lifetimeSeconds = checked((int)lifetime.TotalSeconds);
                var generatedAt = DateTimeOffset.UtcNow;

                var url = await _externalClient.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectKey)
                        .WithExpiry(lifetimeSeconds));

                return new TemporaryObjectUrl(
                    url,
                    generatedAt.AddSeconds(lifetimeSeconds));
            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
            catch (Exception exception)
                when (exception is not OperationCanceledException)
            {
                _logger.LogError(
                    exception,
                    "MinIO presigned URL creation failed for bucket {BucketName} and object {ObjectKey}.",
                    _options.BucketName,
                    objectKey);

                throw new ObjectStorageUnavailableException(
                    "Object storage is unavailable.",
                    exception);
            }
        }

        private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
        {
            var bucketExists = await _internalClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_options.BucketName),
                cancellationToken);

            if (!bucketExists)
            {
                await _internalClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_options.BucketName),
                    cancellationToken);
            }
        }
    }
}
