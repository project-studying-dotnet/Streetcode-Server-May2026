using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Validators.Users;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Users
{
    public class GoogleLoginRequestValidatorTests
    {
        private readonly GoogleLoginRequestValidator _validator;

        public GoogleLoginRequestValidatorTests()
        {
            _validator = new GoogleLoginRequestValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_IdToken_Is_Empty(string? token)
        {
            var model = new GoogleLoginRequest { IdToken = token! };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.IdToken);
        }

        [Fact]
        public void Should_Have_Error_When_IdToken_Is_Too_Short()
        {
            var model = new GoogleLoginRequest { IdToken = "short-token" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.IdToken);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IdToken_Is_Valid_Length()
        {
            var model = new GoogleLoginRequest { IdToken = new string('a', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
