using FluentValidation;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Team.TeamMembersLinks.Create
{
    /// <summary>
    /// Validator for <see cref="TeamMemberLinkDTO"/>.
    /// </summary>
    public class TeamMemberLinkDtoValidator : AbstractValidator<TeamMemberLinkDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamMemberLinkDtoValidator"/> class.
        /// </summary>
        public TeamMemberLinkDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .WithMessage(ErrorMessages.TargetUrlIsRequired)
                .Must(BeAValidUrl)
                .WithMessage(ErrorMessages.TargetUrlMustBeValid);

            RuleFor(x => x.TeamMemberId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.TeamMemberIdMustBePositive);

            RuleFor(x => x.LogoType)
                .IsInEnum()
                .WithMessage(ErrorMessages.InvalidLogoType);
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}