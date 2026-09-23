using MovieApi.Core.Security;
using System.IdentityModel.Tokens.Jwt;

namespace MovieApi.Web.Security
{
    public sealed class HttpCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var subject = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(JwtRegisteredClaimNames.Sub)?
                    .Value;

                return Guid.TryParse(subject, out var userId)
                    ? userId
                    : null;
            }
        }
    }
}
