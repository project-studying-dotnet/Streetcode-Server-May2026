using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.Create
{
    public class CreateSourceLinkCategoryCommandValidator
        : AbstractValidator<CreateSourceLinkCategoryCommand>
    {
        private const int MaxTitleLength = 23;
        private const string CaseInsensitiveCollation = "Ukrainian_CI_AS";

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
                            .GetFirstOrDefaultAsync(
                                c => EF.Functions.Collate(c.Title!, CaseInsensitiveCollation) == title,
                                cancellationToken: cancellationToken) is null)
                    .WithMessage(ErrorMessages.SourceCategoryAlreadyExists);

                RuleFor(x => x.Category.ImageId)
                    .GreaterThan(0);
            });
        }
    }
}