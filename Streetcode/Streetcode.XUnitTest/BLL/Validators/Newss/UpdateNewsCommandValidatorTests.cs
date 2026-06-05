using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.BLL.Validators.Newss.Update;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Newss.Update
{
    public class UpdateNewsCommandValidatorTests
    {
        private readonly UpdateNewsCommandValidator _validator;

        public UpdateNewsCommandValidatorTests()
        {
            _validator = new UpdateNewsCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_News_Is_Null()
        {
            var command = new UpdateNewsCommand(null!);

            Assert.Throws<NullReferenceException>(() =>
            {
                _validator.TestValidate(command);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_News_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var newsDto = CreateValidNewsDto();
            newsDto.Id = invalidId;
            var command = new UpdateNewsCommand(newsDto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.news.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var validNewsDto = CreateValidNewsDto();
            var command = new UpdateNewsCommand(validNewsDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static NewsDTO CreateValidNewsDto()
        {
            return new NewsDTO
            {
                Id = 1,
                Title = "Валідна новина для оновлення",
                Text = "Текст новини з достатньою довжиною для успішної валідації.",
                URL = "valid-news-url",
                ImageId = 1
            };
        }
    }
}
