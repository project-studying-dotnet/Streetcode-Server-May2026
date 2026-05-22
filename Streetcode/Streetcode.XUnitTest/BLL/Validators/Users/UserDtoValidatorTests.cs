using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Validators.Users;
using Xunit;
using Streetcode.DAL.Enums;

namespace Streetcode.XUnitTest.Validators.Users
{
    public class UserDtoValidatorTests
    {
        private readonly UserDtoValidator _validator;

        public UserDtoValidatorTests()
        {
            _validator = new UserDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_Is_Empty(string invalidName)
        {
            var dto = CreateValidDto();
            dto.Name = invalidName;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_50_Characters()
        {
            var dto = CreateValidDto();
            dto.Name = new string('a', 51);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must not exceed 50 characters");
        }

        [Theory]
        [InlineData("plainaddress")] 
        [InlineData("missing-at.com")] 
        [InlineData("username@missingdomain")]  
        public void Should_Have_Error_When_Email_Is_Invalid(string invalidEmail)
        {
            var dto = CreateValidDto();
            dto.Email = invalidEmail;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email format");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Exceeds_20_Characters()
        {
            var dto = CreateValidDto();
            dto.Password = new string('p', 21);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must not exceed 20 characters");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private UserDTO CreateValidDto()
        {
            return new UserDTO
            {
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                Login = "johndoe",
                Password = "Password123",
                Role = UserRole.Moderator
            };
        }
    }
}