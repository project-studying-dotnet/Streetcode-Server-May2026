using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.Validators.AdditionalContent.Tag.GetTagByTitle;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Tag.GetTagByTitle
{
    public class GetTagByTitleQueryValidatorTests
    {
        private readonly GetTagByTitleQueryValidator _validator;

        public GetTagByTitleQueryValidatorTests()
        {
            _validator = new GetTagByTitleQueryValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Title_Is_Empty_Or_Null(string? invalidTitle)
        {
            var query = new GetTagByTitleQuery(invalidTitle!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\n")]
        public void Should_Have_Error_When_Title_Is_Empty_Or_Whitespace(string? invalidTitle)
        {
            var query = new GetTagByTitleQuery(invalidTitle!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
        {
            var longTitle = new string('A', 101);
            var query = new GetTagByTitleQuery(longTitle);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title must not exceed 100 characters");
        }

        [Theory]
        [InlineData("Культура")]
        [InlineData("А")]
        [InlineData("This is a valid tag title with exactly 100 characters sentence length testing example context word")]
        public void Should_Not_Have_Errors_When_Title_Is_Valid(string validTitle)
        {
            var query = new GetTagByTitleQuery(validTitle);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}