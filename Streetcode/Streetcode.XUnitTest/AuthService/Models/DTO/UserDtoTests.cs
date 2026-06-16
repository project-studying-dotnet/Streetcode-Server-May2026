using System.ComponentModel.DataAnnotations;
using Streetcode.Auth.Models.DTO;
using Xunit;
using FluentAssertions;
using Streetcode.Auth.Validators.Users;

namespace Streetcode.XUnitTest.AuthService.Models.DTO
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_Validation_ShouldFail_WhenNameIsTooLong()
        {
            var validator = new UserDtoValidator();
            var dto = new UserDto
            {
                Name = new string('A', 51),
                Surname = "Test",
                Email = "test@test.com",
                Login = "login"
            };

            var result = validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }
}
