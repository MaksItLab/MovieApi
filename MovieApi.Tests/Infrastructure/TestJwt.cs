namespace MovieApi.Tests.Infrastructure
{
    internal static class TestJwt
    {
        public const string Issuer = "MovieApi.Tests";
        public const string Audience = "MovieApi.Tests.Client";
        public const string SigningKey =
            "movie-api-tests-signing-key-with-at-least-32-bytes";

        public static IReadOnlyDictionary<string, string?> Configuration { get; } =
            new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:SigningKey"] = SigningKey,
                ["Jwt:LifetimeMinutes"] = "60",
                ["ConnectionStrings:Postgres"] =
                    "Host=unused;Database=movie_api_tests;Username=unused;Password=unused"
            };
    }
}
