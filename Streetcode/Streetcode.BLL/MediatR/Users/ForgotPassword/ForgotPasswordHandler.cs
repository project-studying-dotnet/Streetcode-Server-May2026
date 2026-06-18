using System.Web;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Users.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailPublisher _rabbitPublisher;
        private readonly ILocalEmailPublisher _localPublisher;

        public ForgotPasswordHandler(
            UserManager<User> userManager,
            IEmailPublisher rabbitPublisher,
            ILocalEmailPublisher localPublisher)
        {
            _userManager = userManager;
            _rabbitPublisher = rabbitPublisher;
            _localPublisher = localPublisher;
        }

        public async Task<Result<Unit>> Handle(ForgotPasswordCommand request, CancellationToken сancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.ForgotPasswordDto.Email);

            if (user == null)
            {
                return Result.Ok(Unit.Value);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = HttpUtility.UrlEncode(token);

            var link = $"http://localhost:3000/admin-panel/reset-password?token={encodedToken}&email={user.Email}";
            var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #2c3e50;'>Скидання пароля до Streetcode</h2>
                    <p>Вітаємо!</p>
                    <p>Ви отримали цей лист, тому що надійшов запит на зміну пароля для вашого облікового запису.</p>
                    <p>Для того, щоб створити новий пароль, натисніть на кнопку нижче:</p>
                    <a href='{link}' style='background-color: #3498db; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                        Відновити пароль
                    </a>
                    <p style='margin-top: 20px; font-size: 12px; color: #7f8c8d;'>
                        Якщо ви не запитували зміну пароля, просто проігноруйте цей лист. Посилання дійсне протягом 24 годин.
                    </p>
                </div>";

            var emailMessage = new EmailMessageContract
            {
                To = new List<string> { user.Email! },
                Subject = "Reset Password",
                Content = emailBody
            };

            try
            {
                await _rabbitPublisher.PublishAsync(emailMessage, сancellationToken);
            }
            catch (Exception)
            {
                await _localPublisher.PublishAsync(emailMessage, сancellationToken);
            }

            return Result.Ok(Unit.Value);
        }
    }
}
