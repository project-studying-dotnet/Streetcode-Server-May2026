using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Streetcode.BLL.MediatR.Behaviors;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators
{
    public class ValidationBehaviorTests
    {
        public class TestRequest : IRequest<Result<string>> { }

        [Fact]
        public async Task Should_Return_Errors_When_Validation_Fails()
        {
            var failure = new ValidationFailure("PropertyName", "Error message")
            {
                ErrorCode = "ErrorCode"
            };

            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
                .ReturnsAsync(new ValidationResult(new[] { failure }));

            var behavior = new ValidationBehavior<TestRequest, Result<string>>(
                new[] { validatorMock.Object });

            var nextCalled = false;

            RequestHandlerDelegate<Result<string>> next = () =>
            {
                nextCalled = true;
                return Task.FromResult(Result.Ok("Success"));
            };

            var result = await behavior.Handle(new TestRequest(), next, default);

            result.IsFailed.Should().BeTrue();

            result.Errors.Should().NotBeEmpty();

            var error = result.Errors.First();

            error.Message.Should().Be("Error message");
            error.Metadata["PropertyName"].Should().Be("PropertyName");
            error.Metadata["ErrorCode"].Should().Be("ErrorCode");

            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Call_Next_When_Validation_Succeeds()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
                .ReturnsAsync(new ValidationResult());

            var nextMock = new Mock<RequestHandlerDelegate<Result<string>>>();

            nextMock
                .Setup(n => n())
                .ReturnsAsync(Result.Ok("Success"));

            var behavior = new ValidationBehavior<TestRequest, Result<string>>(
                new[] { validatorMock.Object });

            var result = await behavior.Handle(new TestRequest(), nextMock.Object, default);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Success");

            nextMock.Verify(n => n(), Times.Once);
        }
    }
}
