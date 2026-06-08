using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using Streetcode.BLL.Validators.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.Validators.Timeline.HistoricalContext;

public sealed class GetAllHistoricalContextQueryValidatorTests
{
    private GetAllHistoricalContextQueryValidator Validator { get; } = new();

    [Fact]
    public async Task Validate_ShouldAlwaysReturnSuccess()
    {
        // Arrange
        GetAllHistoricalContextQuery query = new();

        // Act
        TestValidationResult<GetAllHistoricalContextQuery> result = Validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}