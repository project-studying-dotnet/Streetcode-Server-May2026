using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.RelatedTerm.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedTerm.Create
{
    public class CreateRelatedTermCommandValidatorTests
    {
        private readonly CreateRelatedTermCommandValidator _validator;

        public CreateRelatedTermCommandValidatorTests()
        {
            _validator = new CreateRelatedTermCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_RelatedTerm_Is_Null()
        {
            var command = new CreateRelatedTermCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateRelatedTerm)
                  .WithErrorMessage(ErrorMessages.RelatedTermIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var validRelatedTermDto = new CreateRelatedTermDto
            {
                Word = "Валідний термін",
                TermId = 1,
            };

            var command = new CreateRelatedTermCommand(validRelatedTermDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
