using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class GetTimelineItemsByStreetcodeIdQueryValidatorTests
{
    private GetTimelineItemsByStreetcodeIdQueryValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldReturnError_WhenIdIsLessThanZero()
    {
        // Arrange
        GetTimelineItemsByStreetcodeIdQuery query = new(-1);

        // Act
        TestValidationResult<GetTimelineItemsByStreetcodeIdQuery> result = Validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.StreetcodeId);
    }
}