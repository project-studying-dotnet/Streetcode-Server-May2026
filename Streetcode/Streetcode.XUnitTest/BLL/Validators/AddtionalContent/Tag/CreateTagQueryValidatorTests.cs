using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.AdditionalContent.Tag.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Tag.Create
{
    public class CreateTagQueryValidatorTests
    {
        private readonly CreateTagQueryValidator _validator;

        public CreateTagQueryValidatorTests()
        {
            _validator = new CreateTagQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Tag_Is_Null()
        {
            var query = new CreateTagQuery(null!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.tag);
        }

        [Fact]
        public void Should_Have_Error_When_Inner_Tag_Title_Is_Empty()
        {
            var invalidTagDto = new CreateTagDTO { Title = "" };
            var query = new CreateTagQuery(invalidTagDto);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.tag.Title)
                 .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_Query_Is_Fully_Valid()
        {
            var validTagDto = new CreateTagDTO { Title = "Мистецтво" };
            var query = new CreateTagQuery(validTagDto);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
