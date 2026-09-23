using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Contracts.Auth;
using MovieApi.Core.Auth;
using MovieApi.Domain.Entities;
using MovieApi.Web.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieApi.Web.Security
{
    public class JwtAccessTokenService : IAccessTokenService
    {
        private readonly JwtOptions _options;

        public JwtAccessTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public AccessToken Create(User user)
        {
            var issuedAtUtc = DateTime.UtcNow;
            var expiresAtUtc = issuedAtUtc.AddMinutes(_options.LifetimeMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString("D")),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(MovieApiClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D"))
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SigningKey));

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: issuedAtUtc,
                expires: expiresAtUtc,
                signingCredentials: new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.HmacSha256));

            var value = new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessToken(value, expiresAtUtc);
        }
    }
}
