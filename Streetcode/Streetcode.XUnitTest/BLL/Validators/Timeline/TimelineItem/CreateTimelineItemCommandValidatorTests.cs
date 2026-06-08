using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class CreateTimelineItemCommandValidatorTests
{
    private CreateTimelineItemCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDtoIsInvalid()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = -1,
            Title = ""
        };
        CreateTimelineItemCommand command = new(dto);

        // Act
        TestValidationResult<CreateTimelineItemCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }
}