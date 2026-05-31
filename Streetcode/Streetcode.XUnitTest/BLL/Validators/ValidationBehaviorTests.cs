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
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
                .ReturnsAsync(new ValidationResult(new[] { failure }));

            var nextMock = new Mock<RequestHandlerDelegate<Result<string>>>();

            var behavior = new ValidationBehavior<TestRequest, Result<string>>(new[] { validatorMock.Object });

            var result = await behavior.Handle(new TestRequest(), nextMock.Object, default);

            result.IsFailed.Should().BeTrue();
            result.Reasons.Should().ContainSingle();
            result.Reasons[0].Message.Should().Be("Error message");
            result.Reasons[0].Metadata["PropertyName"].Should().Be("PropertyName");

            nextMock.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Should_Call_Next_When_Validation_Succeeds()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
                .ReturnsAsync(new ValidationResult());

            var nextMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            nextMock.Setup(n => n()).ReturnsAsync(Result.Ok("Success"));

            var behavior = new ValidationBehavior<TestRequest, Result<string>>(new[] { validatorMock.Object });

            var result = await behavior.Handle(new TestRequest(), nextMock.Object, default);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Success");
            nextMock.Verify(n => n(), Times.Once);
        }
    }
}