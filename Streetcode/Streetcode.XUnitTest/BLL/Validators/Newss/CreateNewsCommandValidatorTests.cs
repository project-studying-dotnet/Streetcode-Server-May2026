using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.BLL.Validators.Newss.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Newss.Create
{
    public class CreateNewsCommandValidatorTests
    {
        private readonly CreateNewsCommandValidator _validator;

        public CreateNewsCommandValidatorTests()
        {
            _validator = new CreateNewsCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_NewNews_Is_Null()
        {
            var command = new CreateNewsCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.newNews)
                  .WithErrorMessage("News is required");
        }
        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var validNewsDto = new NewsDTO
            {
                Title = "Валідна новина",
                Text = "Текст новини з достатньою довжиною для успішної валідації.",
                URL = "valid-news-url",
                ImageId = 1
            };
            var command = new CreateNewsCommand(validNewsDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}