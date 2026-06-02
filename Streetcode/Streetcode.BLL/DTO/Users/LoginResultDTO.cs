using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDto
    {
        required public UserDto User { get; set; }
        required public string Token { get; set; }
        required public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
