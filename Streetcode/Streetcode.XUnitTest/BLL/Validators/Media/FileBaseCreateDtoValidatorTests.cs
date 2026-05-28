using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media;
using Streetcode.BLL.Validators.Media;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media
{
    public class FileBaseCreateDtoValidatorTests
    {
        private readonly FileBaseCreateDtoValidator _validator;

        public FileBaseCreateDtoValidatorTests()
        {
            _validator = new FileBaseCreateDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Title_Is_Empty(string? invalidTitle)
        {
            var dto = CreateValidDto();
            dto.Title = invalidTitle;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Title = new string('A', 256);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_BaseFormat_Is_Empty(string? invalidBaseFormat)
        {
            var dto = CreateValidDto();
            dto.BaseFormat = invalidBaseFormat;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.BaseFormat);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_MimeType_Is_Empty(string? invalidMimeType)
        {
            var dto = CreateValidDto();
            dto.MimeType = invalidMimeType;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.MimeType);
        }

        [Fact]
        public void Should_Have_Error_When_MimeType_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.MimeType = new string('A', 101);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.MimeType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Extension_Is_Empty(string? invalidExtension)
        {
            var dto = CreateValidDto();
            dto.Extension = invalidExtension;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Extension);
        }

        [Fact]
        public void Should_Have_Error_When_Extension_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Extension = new string('A', 11);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Extension);
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
            var dto = new FileBaseCreateDTO
            {
                Title = new string('A', 255),
                BaseFormat = "png",
                MimeType = new string('B', 100),
                Extension = new string('C', 10)
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static FileBaseCreateDTO CreateValidDto()
        {
            return new FileBaseCreateDTO
            {
                Title = "valid_file_title",
                BaseFormat = "mp3",
                MimeType = "audio/mpeg",
                Extension = ".mp3"
            };
        }
    }
}