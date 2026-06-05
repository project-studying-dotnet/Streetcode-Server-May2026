using FluentValidation;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.Delete
{
    public class DeleteSourceLinkCategoryCommandValidator
        : AbstractValidator<DeleteSourceLinkCategoryCommand>
    {
        public DeleteSourceLinkCategoryCommandValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.SourceCategoryIdRequired);

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                    await repositoryWrapper.SourceCategoryRepository
                        .GetFirstOrDefaultAsync(
                            c => c.Id == id,
                            cancellationToken: cancellationToken) is not null)
                .WithMessage(ErrorMessages.SourceCategoryNotFound);
        }
    }
}