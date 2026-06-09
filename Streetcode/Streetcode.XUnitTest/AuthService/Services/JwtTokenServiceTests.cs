using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Users;
using Streetcode.Common.Configuration;
using Xunit;

public class JwtTokenServiceTests
{
    private JwtSettings GetSettings() => new JwtSettings
    {
        Key = "THIS_IS_A_VERY_LONG_TEST_SECRET_KEY_123456789",
        Issuer = "test-issuer",
        Audience = "test-audience",
        AccessTokenLifetimeInMinutes = 60
    };

    private static User GetUser() => new User
    {
        Id = 1,
        UserName = "testuser",
        Role = Streetcode.Common.Enums.UserRole.Administrator,
        Name = "Test",
        Surname = "User"
    };

    private string WriteToken(JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private JwtSecurityToken ReadJwt(string token, JwtSettings settings)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(settings.Key))
        };

        handler.ValidateToken(token, validationParameters, out var validatedToken);

        return (JwtSecurityToken)validatedToken;
    }

    [Fact]
    public void GenerateToken_ShouldCreateValidJwtToken()
    {
        var settings = GetSettings();
        var service = new JwtTokenService(settings);
        var user = GetUser();

        var token = service.GenerateToken(user);
        var jwtString = WriteToken(token);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtString);

        token.Should().NotBeNull();
        token.Issuer.Should().Be(settings.Issuer);

        jwt.Claims.Should().Contain(c =>
            c.Type == "aud" && c.Value == settings.Audience);
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        var settings = GetSettings();
        var service = new JwtTokenService(settings);
        var user = GetUser();

        var token = service.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler()
            .ReadJwtToken(WriteToken(token));

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Name && c.Value == user.UserName);

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldHaveExpiration()
    {
        var settings = GetSettings();
        var service = new JwtTokenService(settings);
        var user = GetUser();

        var token = service.GenerateToken(user);

        token.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_ShouldBeCryptographicallyValid()
    {
        var settings = GetSettings();
        var service = new JwtTokenService(settings);
        var user = GetUser();

        var token = service.GenerateToken(user);

        var jwt = ReadJwt(WriteToken(token), settings);

        jwt.Should().NotBeNull();
        jwt.Claims.Should().NotBeEmpty();
    }
}