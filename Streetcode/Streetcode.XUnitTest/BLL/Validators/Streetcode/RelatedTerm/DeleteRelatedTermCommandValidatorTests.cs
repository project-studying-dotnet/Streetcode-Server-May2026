using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.Validators.Streetcode.RelatedTerm.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedTerm.Delete
{
    public class DeleteRelatedTermCommandValidatorTests
    {
        private readonly DeleteRelatedTermCommandValidator _validator;

        public DeleteRelatedTermCommandValidatorTests()
        {
            _validator = new DeleteRelatedTermCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Word_Is_Empty(string? invalidWord)
        {
            var command = new DeleteRelatedTermCommand(invalidWord!, 1);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Word);
        }

        [Fact]
        public void Should_Have_Error_When_Word_Exceeds_Maximum_Length()
        {
            var command = new DeleteRelatedTermCommand(new string('a', 256), 1);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Word);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_TermId_Is_Not_Positive(int invalidTermId)
        {
            var command = new DeleteRelatedTermCommand("Валідний термін", invalidTermId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TermId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            var command = new DeleteRelatedTermCommand("Валідний термін", 1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Word_Is_Exactly_At_Maximum_Length()
        {
            var command = new DeleteRelatedTermCommand(new string('a', 255), 1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}