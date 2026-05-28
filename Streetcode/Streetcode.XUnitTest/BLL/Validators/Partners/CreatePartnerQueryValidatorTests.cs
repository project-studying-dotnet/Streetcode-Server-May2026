using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Validators.Partners.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.Create
{
    public class CreatePartnerQueryValidatorTests
    {
        private readonly CreatePartnerQueryValidator _validator;

        public CreatePartnerQueryValidatorTests()
        {
            _validator = new CreatePartnerQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_NewPartner_Is_Null()
        {
            var query = new CreatePartnerQuery(null!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.newPartner)
                  .WithErrorMessage("Partner is required");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Query_Is_Fully_Valid()
        {
            var validPartnerDto = new CreatePartnerDTO
            {
                Title = "Валідний Партнер",
                LogoId = 1,
                Description = "Короткий опис",
                TargetUrl = "https://streetcode.ua",
                UrlTitle = "Посилання",
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>()
            };
            var query = new CreatePartnerQuery(validPartnerDto);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}