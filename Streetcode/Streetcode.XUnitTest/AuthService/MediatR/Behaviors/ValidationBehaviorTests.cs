using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using Streetcode.Auth.MediatR.Behaviors;
using FluentValidation.Results;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Behaviors
{
    public class ValidationBehaviorTests
    {
        public class TestRequest : IRequest<string>
        {
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public async Task Handle_WhenNoValidators_ShouldCallNext()
        {
            var validators = new List<IValidator<TestRequest>>();

            var behavior = new ValidationBehavior<TestRequest, string>(validators);

            var request = new TestRequest();

            var nextCalled = false;

            RequestHandlerDelegate<string> next = () =>
            {
                nextCalled = true;
                return Task.FromResult("OK");
            };

            var result = await behavior.Handle(request, next, CancellationToken.None);

            result.Should().Be("OK");
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowException()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();

            validatorMock
                .Setup(v => v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
            new ValidationFailure("Name", "Required")
                }));

            var validators = new List<IValidator<TestRequest>>
    {
        validatorMock.Object
    };

            var behavior = new ValidationBehavior<TestRequest, string>(validators);

            var request = new TestRequest();

            RequestHandlerDelegate<string> next = () => Task.FromResult("OK");

            Func<Task> act = () =>
                behavior.Handle(request, next, CancellationToken.None);

            await act.Should()
                .ThrowAsync<Streetcode.Shared.Web.Exceptions.ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenValidationPasses_ShouldCallNext()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();

            validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var validators = new List<IValidator<TestRequest>>
        {
            validatorMock.Object
        };

            var behavior = new ValidationBehavior<TestRequest, string>(validators);

            var request = new TestRequest();

            var nextCalled = false;

            RequestHandlerDelegate<string> next = () =>
            {
                nextCalled = true;
                return Task.FromResult("OK");
            };

            var result = await behavior.Handle(request, next, CancellationToken.None);

            result.Should().Be("OK");
            nextCalled.Should().BeTrue();
        }
    }
}