using System.ComponentModel.DataAnnotations;
using Streetcode.Auth.Models.DTO;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.AuthService.Models.DTO
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_Validation_ShouldFail_WhenNameIsTooLong()
        {
            var dto = new UserDto
            {
                Name = new string('A', 51),
                Surname = "Test",
                Email = "test@test.com",
                Login = "login"
            };

            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains("Name"));
        }
    }
}
