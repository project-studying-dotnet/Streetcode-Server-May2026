using FluentValidation;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Update
{
    public class UpdateStreetcodeCategoryContentCommandValidator
        : AbstractValidator<UpdateStreetcodeCategoryContentCommand>
    {
        public UpdateStreetcodeCategoryContentCommandValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.CategoryContent)
                .NotNull();

            When(x => x.CategoryContent is not null, () =>
            {
                RuleFor(x => x.CategoryContent.Text)
                    .NotEmpty()
                    .MaximumLength(4000);

                RuleFor(x => x.CategoryContent.StreetcodeId)
                    .GreaterThan(0);

                RuleFor(x => x.CategoryContent.SourceLinkCategoryId)
                    .GreaterThan(0);

                RuleFor(x => x.CategoryContent)
                    .MustAsync(async (dto, cancellationToken) =>
                        await repositoryWrapper.StreetcodeCategoryContentRepository
                            .GetFirstOrDefaultAsync(c =>
                                c.StreetcodeId == dto.StreetcodeId &&
                                c.SourceLinkCategoryId == dto.SourceLinkCategoryId) is not null)
                    .WithMessage(ErrorMessages.SourceCategoryNotFound);
            });
        }
    }
}