using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Users;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Users
{
    public class UserLoginDtoValidatorTests
    {
        private const int MaxLoginLength = 20;
        private const int MaxPasswordLength = 20;

        private readonly UserLoginDtoValidator _validator;

        public UserLoginDtoValidatorTests()
        {
            _validator = new UserLoginDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Login_Is_Empty(string? invalidLogin)
        {
            var dto = CreateValidDto();
            dto.Login = invalidLogin!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Login)
                  .WithErrorMessage(ErrorMessages.LoginIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Login_Exceeds_20_Characters()
        {
            var dto = CreateValidDto();
            dto.Login = new string('a', 21);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Login)
                  .WithErrorMessage(string.Format(ErrorMessages.LoginMustNotExceedCharacters, MaxLoginLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Password_Is_Empty(string? invalidPassword)
        {
            var dto = CreateValidDto();
            dto.Password = invalidPassword!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(ErrorMessages.PasswordIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Exceeds_20_Characters()
        {
            var dto = CreateValidDto();
            dto.Password = new string('p', 21);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(string.Format(ErrorMessages.PasswordMustNotExceedCharacters, MaxPasswordLength));
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static UserLoginDto CreateValidDto()
        {
            return new UserLoginDto
            {
                Login = "validUser",
                Password = "validPassword123"
            };
        }
    }
}
