using MovieApi.Core.Common;
using MovieApi.Core.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.UploadVideo
{
    public sealed record VerifiedVideoFile(string ContentType, string Extension);

    public static class VideoFileValidator
    {
        public static async Task<OperationResult<VerifiedVideoFile>> ValidateAsync(
            UploadedFile upload,
            CancellationToken cancellationToken)
        {
            if (upload.Length <= 0)
            {
                return OperationResult<VerifiedVideoFile>.Failure(
                    OperationErrorType.Validation,
                    "Video file must not be empty.");
            }

            if (upload.OriginalFileName.Length > 255)
            {
                return OperationResult<VerifiedVideoFile>.Failure(
                    OperationErrorType.Validation,
                    "Video file name must not exceed 255 characters.");
            }

            if (upload.Length > 100 * 1024 * 1024)
            {
                return OperationResult<VerifiedVideoFile>.Failure(
                    OperationErrorType.Validation,
                    "Video file must not exceed 100 MiB.");
            }

            await using var stream = upload.OpenReadStream();
            var header = new byte[12];
            var bytesRead = await stream.ReadAsync(header.AsMemory(), cancellationToken);

            var isMp4 = bytesRead >= 8
                && header.AsSpan(4, 4).SequenceEqual(new byte[] { 0x66, 0x74, 0x79, 0x70 });

            return isMp4
                ? OperationResult<VerifiedVideoFile>.Success(
                    new VerifiedVideoFile("video/mp4", ".mp4"))
                : OperationResult<VerifiedVideoFile>.Failure(
                    OperationErrorType.Validation,
                    "Only MP4 video files are allowed.");
        }
    }
}
