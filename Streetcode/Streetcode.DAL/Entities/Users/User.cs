using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace Streetcode.DAL.Entities.Users
{
    public class User : IdentityUser<int>
    {
        required public string Name { get; set; }
        required public string Surname { get; set; }

        public UserRole Role { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
