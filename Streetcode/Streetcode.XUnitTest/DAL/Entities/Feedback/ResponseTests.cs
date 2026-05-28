using FluentAssertions;
using Streetcode.DAL.Entities.Feedback;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Feedback;

public class ResponseTests
{
    private const int ResponseId = 1;
    private const string Name = "Vasyl";
    private const string Email = "vasyl@test.com";
    private const string Description = "Test description";

    [Fact]
    public void Properties_ShouldSetValuesCorrectly()
    {
        var response = new Response
        {
            Id = ResponseId,
            Name = Name,
            Email = Email,
            Description = Description,
        };

        response.Id.Should().Be(ResponseId);
        response.Name.Should().Be(Name);
        response.Email.Should().Be(Email);
        response.Description.Should().Be(Description);
    }

    [Fact]
    public void OptionalProperties_ShouldAllowNull()
    {
        var response = new Response
        {
            Id = ResponseId,
            Email = Email,
            Name = null,
            Description = null,
        };

        response.Name.Should().BeNull();
        response.Description.Should().BeNull();
        response.Email.Should().Be(Email);
    }
}