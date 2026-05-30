using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.GetTagByTitle
{
    public class GetTagByTitleQueryValidator : AbstractValidator<GetTagByTitleQuery>
    {
        private const int MaxTitleLength = 100;
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
