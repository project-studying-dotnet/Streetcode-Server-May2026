using FluentValidation;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Create
{
    public class CreateStreetcodeCategoryContentCommandValidator
        : AbstractValidator<CreateStreetcodeCategoryContentCommand>
    {
        public CreateStreetcodeCategoryContentCommandValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.CategoryContent)
                .NotNull();

            When(x => x.CategoryContent is not null, () =>
            {
                RuleFor(x => x.CategoryContent.Text)
                    .NotEmpty()
                    .MaximumLength(5);

                RuleFor(x => x.CategoryContent.StreetcodeId)
                    .GreaterThan(0)
                    .MustAsync(async (streetcodeId, cancellationToken) =>
                        await repositoryWrapper.StreetcodeRepository
                            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId) is not null)
                    .WithMessage(ErrorMessages.StreetcodeNotFound);

                RuleFor(x => x.CategoryContent.SourceLinkCategoryId)
                    .GreaterThan(0)
                    .MustAsync(async (categoryId, cancellationToken) =>
                        await repositoryWrapper.SourceCategoryRepository
                            .GetFirstOrDefaultAsync(c => c.Id == categoryId) is not null)
                    .WithMessage(ErrorMessages.SourceCategoryNotFound);

                RuleFor(x => x.CategoryContent)
                    .MustAsync(async (dto, cancellationToken) =>
                        await repositoryWrapper.StreetcodeCategoryContentRepository
                            .GetFirstOrDefaultAsync(c =>
                                c.StreetcodeId == dto.StreetcodeId &&
                                c.SourceLinkCategoryId == dto.SourceLinkCategoryId) is null)
                    .WithMessage(ErrorMessages.SourceCategoryAlreadyExists);
            });
        }
    }
}