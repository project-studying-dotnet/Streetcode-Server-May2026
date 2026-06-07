using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.TimelineItem;

public sealed class GetAllTimelineItemsQueryValidatorTests
{
    private GetAllTimelineItemsQueryValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldAlwaysReturnSuccess()
    {
        // Arrange
        GetAllTimelineItemsQuery query = new();

        // Act
        TestValidationResult<GetAllTimelineItemsQuery> result = Validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}