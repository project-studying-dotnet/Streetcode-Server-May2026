using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Validators.Users;
using Streetcode.BLL.Resources;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Users
{
    public class ResetPasswordDtoValidatorTests
    {
        private readonly ResetPasswordDtoValidator _validator;

        public ResetPasswordDtoValidatorTests()
        {
            _validator = new ResetPasswordDtoValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new ResetPasswordDto
            {
                Email = "test@example.com",
                Token = "valid-token",
                NewPassword = "Password123"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("short", 8)]
        [InlineData("lowercase123", null)]
        [InlineData("UPPERCASE123", null)]
        [InlineData("NoNumberPass", null)]
        public void Should_Have_Error_When_Password_Is_Invalid(string invalidPassword, int? minLength)
        {
            var model = new ResetPasswordDto
            {
                Email = "test@example.com",
                Token = "token",
                NewPassword = invalidPassword
            };

            var result = _validator.TestValidate(model);

            string expectedMessage = invalidPassword.Length < (minLength ?? 0)
                ? string.Format(ErrorMessages.PasswordMinimumLength, minLength)
                : invalidPassword switch
                {
                    var p when p == "lowercase123" => ErrorMessages.PasswordUppercaseRequired,
                    var p when p == "UPPERCASE123" => ErrorMessages.PasswordLowercaseRequired,
                    var p when p == "NoNumberPass" => ErrorMessages.PasswordNumberRequired,
                    _ => throw new Exception("Unknown test case")
                };

            result.ShouldHaveValidationErrorFor(x => x.NewPassword)
                  .WithErrorMessage(expectedMessage);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new ResetPasswordDto { Email = "invalid-email", Token = "t", NewPassword = "Password123" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(ErrorMessages.InvalidEmailFormat);
        }

        [Fact]
        public void Should_Have_Error_When_Token_Is_Empty()
        {
            var model = new ResetPasswordDto { Email = "test@example.com", Token = "", NewPassword = "Password123" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Token)
                  .WithErrorMessage("Token is required.");
        }
    }
}
