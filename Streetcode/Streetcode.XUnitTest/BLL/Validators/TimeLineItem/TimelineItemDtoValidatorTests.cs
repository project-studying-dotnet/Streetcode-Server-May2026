using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Validators.Timeline.TimelineItem;
using Xunit;
using Streetcode.DAL.Enums;

namespace Streetcode.XUnitTest.Validators.Timeline.TimelineItem
{
    public class TimelineItemDtoValidatorTests
    {
        private readonly TimelineItemDtoValidator _validator;

        public TimelineItemDtoValidatorTests()
        {
            _validator = new TimelineItemDtoValidator();
        }

        [Theory]
        [InlineData(-1)]
        public void Should_Have_Error_When_Id_Is_Less_Than_Zero(int invalidId)
        {
            var dto = CreateValidDto();
            dto.Id = invalidId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Title_Is_Empty(string invalidTitle)
        {
            var dto = CreateValidDto();
            dto.Title = invalidTitle;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_26_Characters()
        {
            var dto = CreateValidDto();
            dto.Title = new string('a', 27);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title must not exceed 26 characters");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_400_Characters()
        {
            var dto = CreateValidDto();
            dto.Description = new string('a', 401);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description must not exceed 400 characters");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Description_Is_Null_Or_Empty()
        {
            var dto = CreateValidDto();
            dto.Description = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private TimelineItemDTO CreateValidDto()
        {
            return new TimelineItemDTO
            {
                Id = 1,
                Title = "Валідний заголовок",
                Description = "Валідний опис",
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                HistoricalContexts = new List<HistoricalContextDTO>()
            };
        }
    }
}