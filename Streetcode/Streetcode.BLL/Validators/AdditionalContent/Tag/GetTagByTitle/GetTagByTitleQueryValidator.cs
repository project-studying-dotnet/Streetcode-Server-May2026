using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.GetTagByTitle
{
    /// <summary>
    /// Validator for GetTagByTitleQuery.
    /// </summary>
    public class GetTagByTitleQueryValidator : AbstractValidator<GetTagByTitleQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTagByTitleQueryValidator"/> class.
        /// </summary>
        public GetTagByTitleQueryValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Title is required")
                .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage("Title cannot contain only whitespace")
                .MaximumLength(100)
                .WithMessage("Title must not exceed 100 characters");
        }
    }
}
