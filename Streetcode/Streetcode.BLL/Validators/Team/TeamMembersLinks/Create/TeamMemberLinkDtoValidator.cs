using FluentValidation;
using Streetcode.BLL.DTO.Team;

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
            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .WithMessage("Target URL is required")
                ////.MaximumLength(2048)
                ////.WithMessage("Target URL must not exceed 2048 characters")
                .Must(BeAValidUrl)
                .WithMessage("Target URL must be a valid URL");

            RuleFor(x => x.TeamMemberId)
                .GreaterThan(0)
                .WithMessage("TeamMemberId must be greater than 0");

            RuleFor(x => x.LogoType)
                .IsInEnum()
                .WithMessage("Invalid logo type");
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}