using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Partners.Create;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.Create
{
    public class CreatePartnerSourceLinkDtoValidatorTests
    {
        private readonly CreatePartnerSourceLinkDtoValidator _validator;

        public CreatePartnerSourceLinkDtoValidatorTests()
        {
            _validator = new CreatePartnerSourceLinkDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_LogoType_Is_Not_A_Valid_Enum_Value()
        {
            var dto = CreateValidDto();

            dto.LogoType = (LogoType)99;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LogoType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_TargetUrl_Is_Empty(string? invalidUrl)
        {
            var dto = CreateValidDto();
            dto.TargetUrl = invalidUrl!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl);
        }

        [Fact]
        public void Should_Have_Error_When_TargetUrl_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            string baseUrl = "https://streetcode.ua/";
            dto.TargetUrl = baseUrl + new string('a', 501 - baseUrl.Length);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl);
        }

        [Theory]
        [InlineData("just-plain-text")]
        [InlineData("www.facebook.com")]
        [InlineData("/relative/route")]
        public void Should_Have_Error_When_TargetUrl_Is_Not_A_Valid_Absolute_URL(string invalidUrl)
        {
            var dto = CreateValidDto();
            dto.TargetUrl = invalidUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl)
                  .WithErrorMessage(ErrorMessages.TargetUrlMustBeValidAbsoluteUrl);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_TargetUrl_Is_Exactly_At_Maximum_Length()
        {
            var dto = CreateValidDto();
            string baseUrl = "https://streetcode.ua/";
            dto.TargetUrl = baseUrl + new string('a', 500 - baseUrl.Length);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static CreatePartnerSourceLinkDTO CreateValidDto()
        {
            return new CreatePartnerSourceLinkDTO
            {
                LogoType = (LogoType)0,
                TargetUrl = "https://facebook.com/streetcodeua"
            };
        }
    }
}