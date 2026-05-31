using FluentValidation;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Delete
{
    public class DeleteStreetcodeCategoryContentValidator
        : AbstractValidator<DeleteStreetcodeCategoryContentCommand>
    {
        public DeleteStreetcodeCategoryContentValidator(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0);

            RuleFor(x => x.SourceLinkCategoryId)
                .GreaterThan(0);

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    await repositoryWrapper.StreetcodeCategoryContentRepository
                        .GetFirstOrDefaultAsync(c =>
                            c.StreetcodeId == request.StreetcodeId &&
                            c.SourceLinkCategoryId == request.SourceLinkCategoryId) is not null)
                .WithMessage(ErrorMessages.SourceCategoryNotFound);
        }
    }
}