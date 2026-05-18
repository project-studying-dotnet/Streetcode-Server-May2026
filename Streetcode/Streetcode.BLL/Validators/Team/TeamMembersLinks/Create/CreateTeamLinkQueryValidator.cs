using FluentValidation;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

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
            RuleFor(x => x.teamMember)
                .NotNull()
                .WithMessage("Team member link is required")
                .SetValidator(new TeamMemberLinkDtoValidator());
        }
    }
}
