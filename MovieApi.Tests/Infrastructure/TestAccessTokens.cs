using Microsoft.IdentityModel.Tokens;
using MovieApi.Domain.Entities;
using MovieApi.Web.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieApi.Tests.Infrastructure
{
    internal static class TestAccessTokens
    {
        public static string Create(UserRole role, string? audience = null)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString("D")),
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
            new Claim(MovieApiClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D"))
        };

            var token = new JwtSecurityToken(
                issuer: TestJwt.Issuer,
                audience: audience ?? TestJwt.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(5),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwt.SigningKey)),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
