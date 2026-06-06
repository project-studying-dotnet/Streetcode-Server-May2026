using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.BLL.Validators.Timeline.TimelineItem.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Timeline.TimelineItem.GetByStreetcodeId
{
    public class GetTimelineItemsByStreetcodeIdQueryValidatorTests
    {
        private readonly GetTimelineItemsByStreetcodeIdQueryValidator _validator;

        public GetTimelineItemsByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetTimelineItemsByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTimelineItemsByStreetcodeIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetTimelineItemsByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
