using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class DeleteTimelineItemCommandValidatorTests
{
    private DeleteTimelineItemCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        DeleteTimelineItemCommand command = new(-1);

        // Act
        TestValidationResult<DeleteTimelineItemCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}