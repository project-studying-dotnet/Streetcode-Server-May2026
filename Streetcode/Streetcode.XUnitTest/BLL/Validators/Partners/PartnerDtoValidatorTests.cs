using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Partners;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners
{
    public class PartnerDtoValidatorTests
    {
        private const int MaxDescriptionLength = 400;
        private readonly PartnerDtoValidator _validator;

        public PartnerDtoValidatorTests()
        {
            _validator = new PartnerDtoValidator();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Than_Zero(int invalidId)
        {
            var dto = CreateValidDto();
            dto.Id = invalidId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage(ErrorMessages.IdMustBeGreaterOrEqualToZero);
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
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_LogoId_Is_Less_Or_Equal_To_Zero(int invalidLogoId)
        {
            var dto = CreateValidDto();
            dto.LogoId = invalidLogoId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LogoId)
                  .WithErrorMessage(ErrorMessages.LogoIdMustBePositive);
        }

        [Fact]
        public void Should_Have_Error_When_TargetUrl_Is_Null()
        {
            var dto = CreateValidDto();
            dto.TargetUrl = null!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TargetUrl)
                  .WithErrorMessage(ErrorMessages.TargetUrlIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_PartnerSourceLinks_Is_Null()
        {
            var dto = CreateValidDto();
            dto.PartnerSourceLinks = null!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PartnerSourceLinks)
                  .WithErrorMessage(ErrorMessages.PartnerSourceLinksRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Streetcodes_Is_Null()
        {
            var dto = CreateValidDto();
            dto.Streetcodes = null!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Streetcodes)
                  .WithErrorMessage(ErrorMessages.StreetcodesRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Description_Is_Exactly_At_Maximum_Length()
        {
            var dto = CreateValidDto();
            dto.Description = new string('A', 400);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Exactly_Zero()
        {
            var dto = CreateValidDto();
            dto.Id = 0;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static PartnerDTO CreateValidDto()
        {
            return new PartnerDTO
            {
                Id = 1,
                Title = "Діючий партнер",
                Description = "Короткий опис наявного партнера платформи.",
                LogoId = 15,
                TargetUrl = new UrlDTO
                {
                    Title = "Офіційний сайт",
                    Href = "https://streetcode.ua"
                },
                PartnerSourceLinks = new List<PartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>()
            };
        }
    }
}
