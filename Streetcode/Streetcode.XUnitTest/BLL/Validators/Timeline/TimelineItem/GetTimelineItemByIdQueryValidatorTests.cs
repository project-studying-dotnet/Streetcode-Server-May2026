using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class GetTimelineItemByIdQueryValidatorTests
{
    private GetTimelineItemByIdQueryValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        GetTimelineItemByIdQuery query = new(-1);

        // Act
        TestValidationResult<GetTimelineItemByIdQuery> result = Validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}