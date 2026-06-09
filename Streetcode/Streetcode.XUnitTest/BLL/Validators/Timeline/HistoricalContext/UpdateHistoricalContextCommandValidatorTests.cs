using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using Streetcode.BLL.Validators.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.HistoricalContext;

public sealed class UpdateHistoricalContextCommandValidatorTests
{
    private UpdateHistoricalContextCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDtoIsInvalid()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = -1,
            Title = ""
        };
        UpdateHistoricalContextCommand command = new(dto);

        // Act
        TestValidationResult<UpdateHistoricalContextCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }
}