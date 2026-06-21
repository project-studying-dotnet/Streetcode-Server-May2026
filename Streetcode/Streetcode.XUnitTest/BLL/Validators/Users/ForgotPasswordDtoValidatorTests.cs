using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Users;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Users
{
    public class ForgotPasswordDtoValidatorTests
    {
        private readonly ForgotPasswordDtoValidator _validator;

        public ForgotPasswordDtoValidatorTests()
        {
            _validator = new ForgotPasswordDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new ForgotPasswordDto { Email = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(ErrorMessages.EmailIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid_Format()
        {
            var model = new ForgotPasswordDto { Email = "invalid-email" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(ErrorMessages.InvalidEmailFormat);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var model = new ForgotPasswordDto { Email = "test@streetcode.com.ua" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
