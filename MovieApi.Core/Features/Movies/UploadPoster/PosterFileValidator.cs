using MovieApi.Core.Common;
using MovieApi.Core.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.UploadPoster
{
    public static class PosterFileValidator
    {
        public static async Task<OperationResult<VerifiedPosterFile>> ValidateAsync(
            UploadedFile upload,
            CancellationToken cancellationToken)
        {
            if (upload.Length <= 0)
            {
                return OperationResult<VerifiedPosterFile>.Failure(
                    OperationErrorType.Validation,
                    "Poster file must not be empty.");
            }

            if (upload.OriginalFileName.Length > 255)
            {
                return OperationResult<VerifiedPosterFile>.Failure(
                    OperationErrorType.Validation,
                    "Poster file name must not exceed 255 characters.");
            }

            if (upload.Length > 5 * 1024 * 1024)
            {
                return OperationResult<VerifiedPosterFile>.Failure(
                    OperationErrorType.Validation,
                    "Poster file must not exceed 5 MiB.");
            }

            await using var stream = upload.OpenReadStream();
            var header = new byte[12];
            var bytesRead = await stream.ReadAsync(header.AsMemory(), cancellationToken);
            var contentType = DetectContentType(header.AsSpan(0, bytesRead));

            return contentType is null
                ? OperationResult<VerifiedPosterFile>.Failure(
                    OperationErrorType.Validation,
                    "Only JPEG, PNG and WebP poster files are allowed.")
                : OperationResult<VerifiedPosterFile>.Success(contentType);
        }

        private static VerifiedPosterFile? DetectContentType(ReadOnlySpan<byte> header)
        {
            if (StartsWith(header, new byte[] { 0xFF, 0xD8, 0xFF }))
            {
                return new VerifiedPosterFile("image/jpeg", ".jpg");
            }

            if (StartsWith(header, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            {
                return new VerifiedPosterFile("image/png", ".png");
            }

            if (header.Length >= 12
                && StartsWith(header, new byte[] { 0x52, 0x49, 0x46, 0x46 })
                && header.Slice(8, 4).SequenceEqual(new byte[] { 0x57, 0x45, 0x42, 0x50 }))
            {
                return new VerifiedPosterFile("image/webp", ".webp");
            }

            return null;
        }

        private static bool StartsWith(ReadOnlySpan<byte> value, ReadOnlySpan<byte> prefix)
        {
            return value.Length >= prefix.Length
                && value[..prefix.Length].SequenceEqual(prefix);
        }
    }

    public sealed record VerifiedPosterFile(string ContentType, string Extension);
}
