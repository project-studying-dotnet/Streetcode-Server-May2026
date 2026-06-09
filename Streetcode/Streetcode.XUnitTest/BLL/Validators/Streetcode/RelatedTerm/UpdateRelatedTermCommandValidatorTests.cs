using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.RelatedTerm.Update;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedTerm.Update
{
    public class UpdateRelatedTermCommandValidatorTests
    {
        private readonly UpdateRelatedTermCommandValidator _validator;

        public UpdateRelatedTermCommandValidatorTests()
        {
            _validator = new UpdateRelatedTermCommandValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new UpdateRelatedTermCommand(invalidId, CreateValidRelatedTermDto());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.id);
        }

        [Fact]
        public void Should_Have_Error_When_RelatedTerm_Is_Null()
        {
            var command = new UpdateRelatedTermCommand(1, null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RelatedTerm)
                .WithErrorMessage(ErrorMessages.RelatedTermIsRequired);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_RelatedTerm_Id_Is_Less_Or_Equal_To_Zero(int invalidRelatedTermId)
        {
            var dto = CreateValidRelatedTermDto();
            dto.Id = invalidRelatedTermId;
            var command = new UpdateRelatedTermCommand(1, dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RelatedTerm.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var command = new UpdateRelatedTermCommand(1, CreateValidRelatedTermDto());

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static RelatedTermDTO CreateValidRelatedTermDto()
        {
            return new RelatedTermDTO
            {
                Id = 1,
                TermId = 1,
                Word = "Валідний термін"
            };
        }
    }
}
