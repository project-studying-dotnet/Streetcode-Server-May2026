using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Users
{
    public class UserRegisterDtoValidatorTests
    {
        private const int MaxNameLength = 50;

        private readonly UserRegisterDtoValidator _validator;

        public UserRegisterDtoValidatorTests()
        {
            _validator = new UserRegisterDtoValidator();
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Surname_Is_Empty(string? invalidSurname)
        {
            var dto = CreateValidDto();
            dto.Surname = invalidSurname!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Surname)
                  .WithErrorMessage(ErrorMessages.SurnameIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Surname_Exceeds_50_Characters()
        {
            var dto = CreateValidDto();
            dto.Name = new string('a', 51);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Surname)
                  .WithErrorMessage(string.Format(ErrorMessages.SurnameMustNotExceedCharacters, MaxNameLength));
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

        [Theory]
        [InlineData("123456789123456789112")]
        public void Should_Have_Error_When_Password_Exceeds_20_Characters(string invalidPassword)
        {
            var dto = CreateValidDto();
            dto.Password = invalidPassword;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(ErrorMessages.PasswordMustNotExceedCharacters);
        }

        [Theory]
        [InlineData("1234567")]
        public void Should_Have_Error_When_Password_Less_Then_8_Characters(string invalidPassword)
        {
            var dto = CreateValidDto();
            dto.Password = invalidPassword;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(ErrorMessages.PasswordMustBeAtLeastCharacters);
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_PasswordConfirmation_Is_Empty(string? invalidPasswordConfirmation)
        {
            var dto = CreateValidDto();
            dto.Password = invalidPasswordConfirmation!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirmation)
                  .WithErrorMessage(ErrorMessages.PasswordConfirmationIsRequired);
        }

        [Theory]
        [InlineData("WrongPass")]
        public void Should_Have_Error_When_PasswordConfirmation_Is_Not_Equal(string invalidPasswordConfirmation)
        {
            var dto = CreateValidDto();
            dto.Password = invalidPasswordConfirmation;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirmation)
                  .WithErrorMessage(ErrorMessages.PasswordsDoNotMatch);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static UserRegisterDto CreateValidDto()
        {
            return new UserRegisterDto
            {
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                Password = "Pass1234",
                PasswordConfirmation = "Pass1234"
            };
        }
    }
}