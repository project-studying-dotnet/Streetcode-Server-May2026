using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Team.TeamMembersLinks.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Team.TeamMembersLinks.Create
{
    public class CreateTeamLinkQueryValidatorTests
    {
        private readonly CreateTeamLinkQueryValidator _validator;

        public CreateTeamLinkQueryValidatorTests()
        {
            _validator = new CreateTeamLinkQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_TeamMember_Is_Null()
        {
            var query = new CreateTeamLinkQuery(null!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.teamMember)
                  .WithErrorMessage(ErrorMessages.TeamMemberLinkIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_TeamMemberDto_Is_Invalid()
        {
            var invalidDto = new TeamMemberLinkDTO { TargetUrl = string.Empty };
            var query = new CreateTeamLinkQuery(invalidDto);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.teamMember.TargetUrl);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_TeamMember_Is_Valid()
        {
            var validDto = new TeamMemberLinkDTO
            {
                Id = 1,
                LogoType = new LogoTypeDTO(),
                TargetUrl = "https://valid-url.com",
                TeamMemberId = 1
            };

            var query = new CreateTeamLinkQuery(validDto);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}