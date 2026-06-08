using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Validators.Timeline;
using Streetcode.DAL.Persistence.Constants;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline;

public sealed class HistoricalContextDtoValidatorTests
{
    private HistoricalContextDtoValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = -1,
            Title = "Valid Title"
        };

        // Act
        TestValidationResult<HistoricalContextDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_ShouldReturnError_WhenTitleIsEmptyOrWhitespaces(string title)
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = -1,
            Title = title
        };

        // Act
        TestValidationResult<HistoricalContextDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTitleExceedCharacters()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = -1,
            Title = new string('A', HistoricalContextConstants.TitleMaxLength + 1)
        };

        // Act
        TestValidationResult<HistoricalContextDto> result = Validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}