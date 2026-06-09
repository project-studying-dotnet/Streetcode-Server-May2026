using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Payment;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Payment;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Payment
{
    public class PaymentDtoValidatorTests
    {
        private readonly PaymentDtoValidator _validator;

        public PaymentDtoValidatorTests()
        {
            _validator = new PaymentDtoValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Amount_Is_Less_Or_Equal_To_Zero(long invalidAmount)
        {
            var dto = CreateValidDto();
            dto.Amount = invalidAmount;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void Should_Have_Error_When_RedirectUrl_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            string baseUrl = "https://streetcode.ua/";
            dto.RedirectUrl = baseUrl + new string('a', 501 - baseUrl.Length);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RedirectUrl);
        }

        [Theory]
        [InlineData("just-plain-text")]
        [InlineData("www.google.com")]
        [InlineData("/relative/path")]
        public void Should_Have_Error_When_RedirectUrl_Is_Provided_But_Not_A_Valid_Absolute_URL(string invalidUrl)
        {
            var dto = CreateValidDto();
            dto.RedirectUrl = invalidUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RedirectUrl)
                  .WithErrorMessage(ErrorMessages.UrlMustBeValidAbsoluteUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Not_Have_Error_When_RedirectUrl_Is_Empty(string? emptyUrl)
        {
            var dto = CreateValidDto();
            dto.RedirectUrl = emptyUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.RedirectUrl);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_RedirectUrl_Is_Exactly_At_Maximum_Length()
        {
            var dto = CreateValidDto();
            string baseUrl = "https://streetcode.ua/";
            dto.RedirectUrl = baseUrl + new string('a', 500 - baseUrl.Length);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static PaymentDTO CreateValidDto()
        {
            return new PaymentDTO
            {
                Amount = 150,
                RedirectUrl = "https://streetcode.ua/success"
            };
        }
    }
}
