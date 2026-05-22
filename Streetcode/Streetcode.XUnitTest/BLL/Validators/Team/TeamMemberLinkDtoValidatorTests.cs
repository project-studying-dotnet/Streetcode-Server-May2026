using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Validators.Team.TeamMembersLinks.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Team.TeamMembersLinks.Create
{
    public class TeamMemberLinkDtoValidatorTests
    {
        private readonly TeamMemberLinkDtoValidator _validator;

        public TeamMemberLinkDtoValidatorTests()
        {
            _validator = new TeamMemberLinkDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-url")]
        public void Should_Have_Error_When_TargetUrl_Is_Invalid(string invalidUrl)
        {
            var dto = CreateValidDto();
            dto.TargetUrl = invalidUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_TeamMemberId_Is_Not_Greater_Than_Zero(int invalidId)
        {
            var dto = CreateValidDto();
            dto.TeamMemberId = invalidId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TeamMemberId)
                  .WithErrorMessage("TeamMemberId must be greater than 0");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private TeamMemberLinkDTO CreateValidDto()
        {
            return new TeamMemberLinkDTO
            {
                TargetUrl = "https://example.com",
                TeamMemberId = 1,
                LogoType = (LogoTypeDTO)0
            };
        }
    }
}