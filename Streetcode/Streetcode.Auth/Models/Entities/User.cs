using Microsoft.AspNetCore.Identity;
using Streetcode.Common.Enums;

namespace Streetcode.Auth.Models.Entities
{
    public class User : IdentityUser<int>
    {
        required public string Name { get; set; }
        required public string Surname { get; set; }

        public UserRole Role { get; set; }
    }
}
