using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using Streetcode.BLL.Validators.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.HistoricalContext;

public sealed class DeleteHistoricalContextCommandValidatorTests
{
    private DeleteHistoricalContextCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        DeleteHistoricalContextCommand command = new(-1);

        // Act
        TestValidationResult<DeleteHistoricalContextCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Validate_ShouldReturnSuccess_WhenIdIsGreaterThanOrEqualToZero(int validId)
    {
        // Arrange
        DeleteHistoricalContextCommand command = new(validId);

        // Act
        TestValidationResult<DeleteHistoricalContextCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Id);
    }
}