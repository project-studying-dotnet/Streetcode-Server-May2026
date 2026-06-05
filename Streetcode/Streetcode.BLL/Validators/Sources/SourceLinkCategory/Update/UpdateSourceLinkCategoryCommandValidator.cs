using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.Update
{
    public class UpdateSourceLinkCategoryCommandValidator
        : AbstractValidator<UpdateSourceLinkCategoryCommand>
    {
        private const int MaxTitleLength = 23;
        private const string CaseInsensitiveCollation = "Ukrainian_CI_AS";

        public UpdateSourceLinkCategoryCommandValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Category)
                .NotNull();

            When(x => x.Category is not null, () =>
            {
                RuleFor(x => x.Category.Id)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessages.SourceCategoryIdRequired);

                RuleFor(x => x.Category)
                    .MustAsync(async (dto, cancellationToken) =>
                        await repositoryWrapper.SourceCategoryRepository
                            .GetFirstOrDefaultAsync(
                                c => c.Id == dto.Id,
                                cancellationToken: cancellationToken) is not null)
                    .WithMessage(ErrorMessages.SourceCategoryNotFound);

                RuleFor(x => x.Category.Title)
                    .NotEmpty()
                    .MaximumLength(MaxTitleLength);

                RuleFor(x => x.Category)
                    .MustAsync(async (dto, cancellationToken) =>
                        await repositoryWrapper.SourceCategoryRepository
                            .GetFirstOrDefaultAsync(
                                c => c.Id != dto.Id &&
                                     EF.Functions.Collate(c.Title!, CaseInsensitiveCollation) == dto.Title,
                                cancellationToken: cancellationToken) is null)
                    .WithMessage(ErrorMessages.SourceCategoryAlreadyExists);

                RuleFor(x => x.Category.ImageId)
                    .GreaterThan(0);
            });
        }
    }
}