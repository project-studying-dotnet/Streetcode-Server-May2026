using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Partners.Update;
using Streetcode.BLL.Validators.Partners.Update;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Streetcode.XUnitTest.Validators.Partners.Update
{
    public class UpdatePartnerQueryValidatorTests
    {
        private readonly UpdatePartnerQueryValidator _validator;

        public UpdatePartnerQueryValidatorTests()
        {
            _validator = new UpdatePartnerQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Partner_Is_Null()
        {
            var query = new UpdatePartnerQuery(null!);

            Assert.Throws<NullReferenceException>(() =>
            {
                _validator.TestValidate(query);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_Partner_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var partnerDto = CreateValidPartnerDto();
            partnerDto.Id = invalidId;
            var query = new UpdatePartnerQuery(partnerDto);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Partner.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Query_Is_Fully_Valid()
        {
            var validPartnerDto = CreateValidPartnerDto();
            var query = new UpdatePartnerQuery(validPartnerDto);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static CreatePartnerDTO CreateValidPartnerDto()
        {
            return new CreatePartnerDTO
            {
                Id = 1,
                Title = "Валідний Партнер для Оновлення",
                LogoId = 5,
                Description = "Опис партнера",
                TargetUrl = "https://streetcode.ua",
                UrlTitle = "Сайт",
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>()
            };
        }
    }
}