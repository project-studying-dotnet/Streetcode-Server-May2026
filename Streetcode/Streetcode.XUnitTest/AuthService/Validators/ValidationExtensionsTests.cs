using FluentAssertions;
using FluentValidation;
using Xunit;
using Streetcode.Auth.Validators;

public class ValidationExtensionsTests
{
    private class TestModel
    {
        public string Value { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    private class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Value)
                .RequiredWithMaxLength(5, "Required", "Max {0}");

            RuleFor(x => x.Email)
                .ValidEmail(10, "Required", "Invalid email", "Max {0}");

            RuleFor(x => x.Password)
                .ValidPassword(3, 10, "Required", "Min {0}", "Max {0}");
        }
    }

    [Fact]
    public void RequiredWithMaxLength_ShouldFail_WhenEmpty()
    {
        var validator = new TestValidator();

        var result = validator.Validate(new TestModel { Value = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == "Required");
    }

    [Fact]
    public void RequiredWithMaxLength_ShouldFail_WhenTooLong()
    {
        var validator = new TestValidator();

        var result = validator.Validate(new TestModel { Value = "123456" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidEmail_ShouldFail_WhenInvalid()
    {
        var validator = new TestValidator();

        var result = validator.Validate(new TestModel { Email = "not-email" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidPassword_ShouldFail_WhenTooShort()
    {
        var validator = new TestValidator();

        var result = validator.Validate(new TestModel { Password = "12" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_ShouldPass_WhenValid()
    {
        var validator = new TestValidator();

        var result = validator.Validate(new TestModel
        {
            Value = "abc",
            Email = "a@b.co",
            Password = "12345"
        });

        result.IsValid.Should().BeTrue();
    }
}