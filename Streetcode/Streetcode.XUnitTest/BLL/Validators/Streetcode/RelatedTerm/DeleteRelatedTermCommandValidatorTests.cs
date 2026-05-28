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
            var command = new DeleteRelatedTermCommand(invalidWord!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.word);
        }

        [Fact]
        public void Should_Have_Error_When_Word_Exceeds_Maximum_Length()
        {
            var command = new DeleteRelatedTermCommand(new string('a', 256));

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.word);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            var command = new DeleteRelatedTermCommand("Валідний термін");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Word_Is_Exactly_At_Maximum_Length()
        {
            var command = new DeleteRelatedTermCommand(new string('a', 255));

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}