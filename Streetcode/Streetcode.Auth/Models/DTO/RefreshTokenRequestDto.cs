using System.Diagnostics.CodeAnalysis;

namespace Streetcode.Auth.Models.DTO
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenRequestDto
    {
        required public string RefreshToken { get; set; }
    }
}