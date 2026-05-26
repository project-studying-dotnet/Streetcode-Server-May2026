using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Streetcode.BLL.Services.Users;
using Streetcode.BLL.Settings;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.Users
{
    public class TokenServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                Key = "StreetcodeSuperSecretJwtKeyForUnitTests1234567890",
                Issuer = "Streetcode.WebApi",
                Audience = "Streetcode.Client",
                AccessTokenLifetimeInMinutes = 120,
            };

            _tokenService = new TokenService(_jwtSettings);
        }

        [Fact]
        public void GenerateJWTToken_ShouldIncludeUserClaims()
        {
            var user = CreateUser();

            var jwtToken = _tokenService.GenerateJWTToken(user);

            jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
            jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());
        }

        [Fact]
        public void GenerateJWTToken_ShouldUseEmptyUserName_WhenUserNameIsNull()
        {
            var user = CreateUser();
            user.UserName = null;

            var jwtToken = _tokenService.GenerateJWTToken(user);

            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == string.Empty);
        }

        [Fact]
        public void RefreshToken_ShouldReturnNewToken_WithSameClaims()
        {
            var user = CreateUser();
            var originalToken = new JwtSecurityTokenHandler().WriteToken(_tokenService.GenerateJWTToken(user));

            var refreshedToken = _tokenService.RefreshToken(originalToken);
            var serializedRefreshedToken = new JwtSecurityTokenHandler().WriteToken(refreshedToken);

            serializedRefreshedToken.Should().NotBeNullOrEmpty();
            refreshedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            refreshedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            refreshedToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());
            refreshedToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void RefreshToken_ShouldThrow_WhenTokenDoesNotContainUserIdentifier()
        {
            var tokenWithoutUserId = CreateTokenWithoutClaim(ClaimTypes.NameIdentifier);

            var act = () => _tokenService.RefreshToken(tokenWithoutUserId);

            act.Should().Throw<SecurityTokenException>()
                .WithMessage("Token does not contain user identifier.");
        }

        [Fact]
        public void RefreshToken_ShouldThrow_WhenTokenDoesNotContainRole()
        {
            var tokenWithoutRole = CreateTokenWithoutClaim(ClaimTypes.Role);

            var act = () => _tokenService.RefreshToken(tokenWithoutRole);

            act.Should().Throw<SecurityTokenException>()
                .WithMessage("Token does not contain user role.");
        }

        private static User CreateUser()
        {
            return new User
            {
                Id = 1,
                UserName = "admin",
                Name = "Admin",
                Surname = "Admin",
                Role = UserRole.MainAdministrator,
            };
        }

        private string CreateTokenWithoutClaim(string claimTypeToExclude)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, UserRole.MainAdministrator.ToString()),
            }.Where(c => c.Type != claimTypeToExclude);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
