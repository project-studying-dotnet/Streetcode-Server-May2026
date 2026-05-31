using FluentValidation;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.Create
{
    public class CreateSourceLinkCategoryCommandValidator
        : AbstractValidator<CreateSourceLinkCategoryCommand>
    {
        private const int MaxTitleLength = 23;

        public CreateSourceLinkCategoryCommandValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Category)
                .NotNull();

            When(x => x.Category is not null, () =>
            {
                RuleFor(x => x.Category.Title)
                    .NotEmpty()
                    .MaximumLength(MaxTitleLength)
                    .MustAsync(async (title, cancellationToken) =>
                        await repositoryWrapper.SourceCategoryRepository
                            .GetFirstOrDefaultAsync(c =>
                                c.Title != null &&
                                c.Title.ToLower() == title.ToLower()) is null)
                    .WithMessage(ErrorMessages.SourceCategoryAlreadyExists);

                RuleFor(x => x.Category.ImageId)
                    .GreaterThan(0);
            });
        }
    }
}