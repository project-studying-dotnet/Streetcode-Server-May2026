using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Users
{
    public class UserDtoValidatorTests
    {
        private const int MaxNameLength = 50;

        private readonly UserDtoValidator _validator;

        public UserDtoValidatorTests()
        {
            _validator = new UserDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_Is_Empty(string? invalidName)
        {
            var dto = CreateValidDto();
            dto.Name = invalidName!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(ErrorMessages.NameIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_50_Characters()
        {
            var dto = CreateValidDto();
            dto.Name = new string('a', 51);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(string.Format(ErrorMessages.NameMustNotExceedCharacters, MaxNameLength));
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missing-at.com")]
        public void Should_Have_Error_When_Email_Is_Invalid(string invalidEmail)
        {
            var dto = CreateValidDto();
            dto.Email = invalidEmail;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(ErrorMessages.InvalidEmailFormat);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static UserDto CreateValidDto()
        {
            return new UserDto
            {
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                Login = "johndoe",
                Role = UserRole.Moderator
            };
        }
    }
}
