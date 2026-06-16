using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.Auth.Models.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; } 
        public string? ReplacedByTokenHash { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
