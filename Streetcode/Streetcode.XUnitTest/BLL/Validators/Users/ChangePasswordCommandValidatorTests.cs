using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.ChangePassword;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Users.ChangePassword;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Users
{
    public class ChangePasswordCommandValidatorTests
    {
        private readonly ChangePasswordCommandValidator _validator;

        public ChangePasswordCommandValidatorTests()
        {
            _validator = new ChangePasswordCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_CurrentPassword_Is_Empty()
        {
            var command = CreateValidCommand();
            command.ChangePasswordRequest.CurrentPassword = string.Empty;

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ChangePasswordRequest.CurrentPassword)
                  .WithErrorMessage(string.Format(ErrorMessages.FieldIsRequired, "Current password"));
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Too_Short()
        {
            var command = CreateValidCommand();
            command.ChangePasswordRequest.NewPassword = "short";

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ChangePasswordRequest.NewPassword)
                  .WithErrorMessage(string.Format(ErrorMessages.PasswordTooShort, ChangePasswordCommandValidator.MinPasswordLength));
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Equals_CurrentPassword()
        {
            var command = CreateValidCommand();
            command.ChangePasswordRequest.CurrentPassword = "Password123";
            command.ChangePasswordRequest.NewPassword = "Password123";

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ChangePasswordRequest.NewPassword)
                  .WithErrorMessage(ErrorMessages.PasswordCannotBeSameAsCurrent);
        }

        [Fact]
        public void Should_Have_Error_When_ConfirmPassword_Does_Not_Match()
        {
            var command = CreateValidCommand();
            command.ChangePasswordRequest.NewPassword = "NewPassword123";
            command.ChangePasswordRequest.ConfirmNewPassword = "DifferentPassword123";

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ChangePasswordRequest.ConfirmNewPassword)
                  .WithErrorMessage(ErrorMessages.PasswordsDoNotMatch);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            var command = CreateValidCommand();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static ChangePasswordCommand CreateValidCommand()
        {
            return new ChangePasswordCommand(1, new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            });
        }
    }
}