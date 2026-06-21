namespace Streetcode.BLL.DTO.Users
{
    public class ResetPasswordDto
    {
        required public string Email { get; set; }
        required public string Token { get; set; }
        required public string NewPassword { get; set; }
    }
}
