using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.GetTagByTitle
{
    /// <summary>
    /// Validator for GetTagByTitleQuery.
    /// </summary>
    public class GetTagByTitleQueryValidator : AbstractValidator<GetTagByTitleQuery>
    {
        private const int MaxTitleLength = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTagByTitleQueryValidator"/> class.
        /// </summary>
        public GetTagByTitleQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired)
                .Must(title => title.Trim().Length > 0)
                .WithMessage(ErrorMessages.TitleCannotBeWhitespace)
                .MaximumLength(MaxTitleLength)
                .WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));
        }
    }
}
