using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Validators.Users;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Users
{
    public class GoogleLoginRequestDtoValidatorTests
    {
        private readonly GoogleLoginRequestDtoValidator _validator;

        public GoogleLoginRequestDtoValidatorTests()
        {
            _validator = new GoogleLoginRequestDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new GoogleLoginRequestDto { Email = "", Name = "Valid", Surname = "Valid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new GoogleLoginRequestDto { Email = "invalid-email", Name = "Valid", Surname = "Valid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Name_Is_Empty(string? name)
        {
            var model = new GoogleLoginRequestDto { Email = "test@test.com", Name = name!, Surname = "Valid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new GoogleLoginRequestDto
            {
                Email = "test@test.com",
                Name = new string('a', 51),
                Surname = "Valid"
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Data_Is_Valid()
        {
            var model = new GoogleLoginRequestDto
            {
                Email = "valid@test.com",
                Name = "John",
                Surname = "Doe"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
