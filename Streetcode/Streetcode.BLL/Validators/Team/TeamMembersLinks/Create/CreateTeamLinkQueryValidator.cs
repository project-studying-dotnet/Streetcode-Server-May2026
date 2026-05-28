using FluentValidation;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Team.TeamMembersLinks.Create
{
    /// <summary>
    /// Validator for <see cref="CreateTeamLinkQuery"/>.
    /// </summary>
    public class CreateTeamLinkQueryValidator : AbstractValidator<CreateTeamLinkQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTeamLinkQueryValidator"/> class.
        /// </summary>
        public CreateTeamLinkQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.teamMember)
                .NotNull()
                .WithMessage(ErrorMessages.TeamMemberLinkIsRequired)
                .SetValidator(new TeamMemberLinkDtoValidator());
        }
    }
}
