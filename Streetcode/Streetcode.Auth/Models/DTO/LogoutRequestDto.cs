using System.Diagnostics.CodeAnalysis;

namespace Streetcode.Auth.Models.DTO
{
    [ExcludeFromCodeCoverage]
    public record LogoutRequestDto(string RefreshToken);
}
