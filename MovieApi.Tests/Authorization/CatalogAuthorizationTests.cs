using MovieApi.Contracts.Movies;
using MovieApi.Domain.Entities;
using MovieApi.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;

namespace MovieApi.Tests.Authorization
{
    public sealed class CatalogAuthorizationTests 
        : IClassFixture<MovieApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CatalogAuthorizationTests(MovieApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetMovies_WithoutToken_ReturnsOk()
        {
            var response = await _client.GetAsync("/movies");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateMovie_WithoutToken_ReturnsUnauthorized()
        {
            var response = await SendCreateMovieAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains(
                response.Headers.WwwAuthenticate,
                value => value.Scheme == "Bearer");
        }

        [Fact]
        public async Task CreateMovie_WithWrongAudience_ReturnsUnauthorized()
        {
            var token = TestAccessTokens.Create(
                UserRole.Admin,
                audience: "Other.MovieApi");

            var response = await SendCreateMovieAsync(token);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateMovie_WithUserRole_ReturnsForbidden()
        {
            var response = await SendCreateMovieAsync(
                TestAccessTokens.Create(UserRole.User));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateMovie_WithAdminRole_ReturnsCreated()
        {
            var response = await SendCreateMovieAsync(
                TestAccessTokens.Create(UserRole.Admin));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        private Task<HttpResponseMessage> SendCreateMovieAsync(
            string? accessToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/movies")
            {
                Content = JsonContent.Create(
                    new CreateMovieRequest(
                        "The Matrix",
                        "Sci-fi movie",
                        1999,
                        136,
                        null,
                        null))
            };

            if (accessToken is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);
            }

            return _client.SendAsync(request);
        }
    }
}
