using Xunit;
using Streetcode.DAL.Enums;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Validators.Timeline;
using Streetcode.DAL.Persistence.Constants;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline;

public sealed class TimelineItemDtoValidatorTests
{
    private TimelineItemDtoValidator Validator { get; } = new();

    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = -1,
            Title = "Title"
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ShouldReturnError_WhenTitleIsEmptyOrWhitespaces(string title)
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = title
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Title);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTitleExceedCharacters()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = new string('A', TimelineItemConstants.TitleMaxLength + 1)
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDescriptionExceedCharacters()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Title",
            Description = new string('A', TimelineItemConstants.DescriptionMaxLength + 1)
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDateIsDefault()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Title",
            Date = default
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDateViewPatternIsInvalidEnum()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Title",
            DateViewPattern = (DateViewPattern)(-1)
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateViewPattern);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenStreetcodeIdIsLessThanZero()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Title",
            StreetcodeId = -1,
        };

        // Act
        TestValidationResult<TimelineItemDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }
}