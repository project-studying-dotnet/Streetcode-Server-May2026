using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Newss;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Newss
{
    public class NewsDtoValidatorTests
    {
        private const int TitleMaxLength = 40;
        private const int TextMaxLength = 450;

        private readonly NewsDtoValidator _validator;

        public NewsDtoValidatorTests()
        {
            _validator = new NewsDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Title_Is_Empty(string? invalidTitle)
        {
            var dto = CreateValidDto();
            dto.Title = invalidTitle!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Title = new string('A', 41);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, TitleMaxLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Text_Is_Empty(string? invalidText)
        {
            var dto = CreateValidDto();
            dto.Text = invalidText!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Text)
                  .WithErrorMessage(ErrorMessages.TextIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Text_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Text = new string('A', 451);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Text)
                  .WithErrorMessage(string.Format(ErrorMessages.TextMustNotExceedCharacters, TextMaxLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_URL_Is_Empty(string? invalidUrl)
        {
            var dto = CreateValidDto();
            dto.URL = invalidUrl!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.URL)
                  .WithErrorMessage(ErrorMessages.UrlIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_ImageId_Is_Null()
        {
            var dto = CreateValidDto();
            dto.ImageId = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.ImageId)
                  .WithErrorMessage(ErrorMessages.ImageIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Properties_Are_Exactly_At_Maximum_Lengths()
        {
            var dto = new NewsDTO
            {
                Title = new string('A', 40),
                Text = new string('B', 450),
                URL = "valid-url",
                ImageId = 1
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static NewsDTO CreateValidDto()
        {
            return new NewsDTO
            {
                Title = "Валідна назва",
                Text = "Короткий, але цілком інформативний текст новини.",
                URL = "https://streetcode.ua/news/some-news",
                ImageId = 42
            };
        }
    }
}
