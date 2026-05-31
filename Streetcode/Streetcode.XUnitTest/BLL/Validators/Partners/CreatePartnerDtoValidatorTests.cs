using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Partners.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.Create
{
    public class CreatePartnerDtoValidatorTests
    {
        private const int MaxDescriptionLength = 400;
        private readonly CreatePartnerDtoValidator _validator;

        public CreatePartnerDtoValidatorTests()
        {
            _validator = new CreatePartnerDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Title_Is_Empty(string? invalidTitle)
        {
            var dto = CreateValidDto();
            dto.Title = invalidTitle!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Description = new string('A', 401);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Not_Have_Error_When_Description_Is_Empty(string? emptyDescription)
        {
            var dto = CreateValidDto();
            dto.Description = emptyDescription;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData("not-a-valid-url")]
        [InlineData("www.google.com")]
        [InlineData("/relative/path")]
        public void Should_Have_Error_When_TargetUrl_Is_Invalid_Absolute_URL(string invalidUrl)
        {
            var dto = CreateValidDto();
            dto.TargetUrl = invalidUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl)
                  .WithErrorMessage(ErrorMessages.TargetUrlMustBeValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Not_Have_Error_When_TargetUrl_Is_Empty(string? emptyUrl)
        {
            var dto = CreateValidDto();
            dto.TargetUrl = emptyUrl;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TargetUrl);
        }

        [Fact]
        public void Should_Have_Error_When_UrlTitle_Exceeds_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.UrlTitle = new string('A', 256);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UrlTitle)
                 .WithErrorMessage(string.Format(ErrorMessages.UrlTitleMustNotExceedCharacters, 255));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_LogoId_Is_Less_Or_Equal_To_Zero(int invalidLogoId)
        {
            var dto = CreateValidDto();
            dto.LogoId = invalidLogoId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LogoId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Optional_Properties_Are_Exactly_At_Maximum_Lengths()
        {
            var dto = new CreatePartnerDTO
            {
                Title = "Валідний партнер",
                Description = new string('A', 400),
                TargetUrl = "https://streetcode.ua",
                UrlTitle = new string('B', 255),
                LogoId = 1,
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>()
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static CreatePartnerDTO CreateValidDto()
        {
            return new CreatePartnerDTO
            {
                Title = "Офіційний партнер",
                Description = "Короткий опис діяльності нашого офіційного партнера проекту.",
                TargetUrl = "https://partner.com",
                UrlTitle = "Перейти на сайт партнера",
                LogoId = 10,
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>()
            };
        }
    }
}