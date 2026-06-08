using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class UpdateTimelineItemCommandValidatorTests
{
    private UpdateTimelineItemCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDtoIsInvalid()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = -1,
            Title = ""
        };
        UpdateTimelineItemCommand command = new(dto);

        // Act
        TestValidationResult<UpdateTimelineItemCommand> result = Validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }
}