namespace MovieApi.Web.Configuration
{
    public static class FileUploadOptions
    {
        public const long MaxPosterSizeBytes = 5 * 1024 * 1024;
        public const long MaxVideoSizeBytes = 100 * 1024 * 1024;
        public const long MultipartOverheadBytes = 1024 * 1024;

        public const long MaxPosterRequestBodySizeBytes =
            MaxPosterSizeBytes + MultipartOverheadBytes;

        public const long MaxVideoRequestBodySizeBytes =
            MaxVideoSizeBytes + MultipartOverheadBytes;
    }
}
